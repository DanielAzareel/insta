# InstaAudit (MVP comercial)

InstaAudit es una aplicación full stack para analizar exportaciones manuales de Instagram (`followers_1.json` y `following.json`) sin pedir credenciales, sin scraping y sin guardar los archivos originales.

## Stack

- **Frontend:** React + Vite + TailwindCSS + React Router + Axios
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Pagos:** Stripe Checkout + Webhook firmado
- **Exportación:** Excel `.xlsx` con ClosedXML
- **Infra objetivo:** VPS Ubuntu + Nginx + systemd

## Decisiones de arquitectura (MVP escalable)

1. **Procesamiento en memoria únicamente**: los archivos se parsean desde stream y nunca se persisten en disco.
2. **Sesión temporal expirable**: resultado en memoria con `analysisToken` y expiración (30 min por defecto).
3. **Paywall robusto del lado backend**: endpoint `full` y `export` validan estado `paid`.
4. **Abstracción de almacenamiento**: `IAnalysisSessionStore` permite reemplazar memoria por Redis sin romper controladores.

## Estructura

```bash
/frontend
/backend
  /src/InstaAudit.Api
```

## Flujo funcional

1. Usuario sube `followers_1.json` y `following.json` + acepta consentimiento.
2. Frontend envía multipart a `POST /api/analysis/preview`.
3. Backend parsea, calcula métricas y responde preview + `analysisToken`.
4. Frontend muestra top 10 y paywall por **$25 MXN**.
5. `POST /api/payments/checkout` crea Stripe Checkout con metadata `analysisToken`.
6. Stripe redirige a `/success?analysisToken=...`.
7. Success valida `GET /api/payments/status/{token}` y carga `GET /api/analysis/full/{token}`.
8. Usuario puede descargar `GET /api/analysis/export/{token}` en Excel.

## Endpoints principales

- `POST /api/analysis/preview`
- `GET /api/analysis/full/{analysisToken}`
- `GET /api/analysis/export/{analysisToken}`
- `POST /api/payments/checkout`
- `GET /api/payments/status/{analysisToken}`
- `POST /api/webhooks/stripe`

## Configuración

### Backend (`backend/src/InstaAudit.Api/appsettings*.json`)

```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "App": {
    "FrontendUrl": "http://localhost:5173",
    "PriceMxn": 25,
    "SessionDurationMinutes": 30
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

### Frontend

Copia `frontend/.env.example` como `.env`.

```bash
VITE_API_BASE_URL=http://localhost:5002
```

## Ejecución local

### 1) Frontend

```bash
cd frontend
npm install
npm run dev
```

### 2) Backend

```bash
cd backend/src/InstaAudit.Api
dotnet restore
dotnet run --urls "http://localhost:5002"
```

Swagger: `http://localhost:5002/swagger`

## Stripe webhook en local

1. Instala Stripe CLI.
2. Loguea: `stripe login`
3. Reenvía eventos:

```bash
stripe listen --forward-to http://localhost:5002/api/webhooks/stripe
```

4. Copia el `whsec_...` mostrado y configúralo en `Stripe:WebhookSecret`.
5. Simula pago (opcional):

```bash
stripe trigger checkout.session.completed
```

## Despliegue VPS Ubuntu (Nginx + systemd)

### Backend como servicio systemd

Publica:

```bash
cd /var/www/instaaudit/backend/src/InstaAudit.Api
dotnet publish -c Release -o /var/www/instaaudit/publish
```

Servicio `/etc/systemd/system/instaaudit-api.service`:

```ini
[Unit]
Description=InstaAudit API
After=network.target

[Service]
WorkingDirectory=/var/www/instaaudit/publish
ExecStart=/usr/bin/dotnet /var/www/instaaudit/publish/InstaAudit.Api.dll --urls http://127.0.0.1:5002
Restart=always
RestartSec=5
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Activación:

```bash
sudo systemctl daemon-reload
sudo systemctl enable instaaudit-api
sudo systemctl start instaaudit-api
sudo systemctl status instaaudit-api
```

### Frontend estático en Nginx

```bash
cd /var/www/instaaudit/frontend
npm ci
npm run build
```

Copia `dist/` a `/var/www/instaaudit/site`.

Nginx `/etc/nginx/sites-available/instaaudit`:

```nginx
server {
    listen 80;
    server_name tu-dominio.com;

    root /var/www/instaaudit/site;
    index index.html;

    location / {
        try_files $uri /index.html;
    }

    location /api/ {
        proxy_pass http://127.0.0.1:5002/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Habilitar:

```bash
sudo ln -s /etc/nginx/sites-available/instaaudit /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### HTTPS con Certbot (posterior)

```bash
sudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d tu-dominio.com
```

## Notas de producción

- Configura `Stripe:SecretKey` y `Stripe:WebhookSecret` reales en variables de entorno/secret manager.
- Ajusta CORS con tu dominio final.
- Para escalar horizontalmente, reemplaza `InMemoryAnalysisSessionStore` por Redis manteniendo la interfaz.
