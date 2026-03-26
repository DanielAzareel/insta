import { useEffect, useMemo, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import ErrorState from '../components/ErrorState';
import LoadingState from '../components/LoadingState';
import { getExportUrl, getFullReport, getPaymentStatus } from '../api/analysisApi';
import type { FullReportResponse } from '../types/api';

export default function SuccessPage() {
  const [params] = useSearchParams();
  const analysisToken = useMemo(() => params.get('analysisToken') ?? '', [params]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [report, setReport] = useState<FullReportResponse | null>(null);

  useEffect(() => {
    const load = async () => {
      if (!analysisToken) {
        setError('No se encontró analysisToken en la URL de éxito.');
        setLoading(false);
        return;
      }

      try {
        const status = await getPaymentStatus(analysisToken);
        if (!status.paid) {
          setError('Tu pago aún no aparece confirmado. Espera unos segundos y recarga.');
          setLoading(false);
          return;
        }

        const full = await getFullReport(analysisToken);
        setReport(full);
      } catch {
        setError('No pudimos recuperar tu reporte completo.');
      } finally {
        setLoading(false);
      }
    };

    void load();
  }, [analysisToken]);

  if (loading) return <main className="mx-auto max-w-5xl p-4"><LoadingState text="Validando pago y cargando reporte..." /></main>;
  if (error) return <main className="mx-auto max-w-5xl p-4"><ErrorState message={error} /></main>;
  if (!report) return null;

  return (
    <main className="mx-auto max-w-6xl space-y-6 px-4 py-8">
      <header className="card text-center space-y-2">
        <h1 className="text-2xl font-bold">Pago aprobado ✅</h1>
        <p className="text-slate-300">Tu reporte completo está listo para consultarse y descargarse.</p>
        <a href={getExportUrl(analysisToken)} className="inline-block rounded-xl bg-emerald-400 px-5 py-2 font-semibold text-slate-900">
          Descargar Excel (.xlsx)
        </a>
      </header>

      <div className="grid gap-4 lg:grid-cols-3">
        <FullList title="No me siguen" users={report.notFollowingBack} />
        <FullList title="Fans" users={report.fans} />
        <FullList title="Mutuos" users={report.mutuals} />
      </div>

      <Link to="/" className="inline-block rounded-lg border border-slate-700 px-4 py-2 text-slate-200">
        Hacer nuevo análisis
      </Link>
    </main>
  );
}

function FullList({ title, users }: { title: string; users: string[] }) {
  return (
    <section className="card">
      <h2 className="font-semibold">{title} ({users.length})</h2>
      <ul className="mt-3 max-h-96 space-y-2 overflow-auto text-sm text-slate-200">
        {users.map((user) => (
          <li key={user} className="rounded border border-slate-800 p-2">@{user}</li>
        ))}
      </ul>
    </section>
  );
}
