interface PaywallCardProps {
  priceMxn: number;
  loading: boolean;
  onCheckout: () => Promise<void>;
}

export default function PaywallCard({ priceMxn, loading, onCheckout }: PaywallCardProps) {
  return (
    <section className="card text-center space-y-4">
      <h3 className="text-xl font-semibold">Desbloquea el reporte completo</h3>
      <p className="text-slate-300">
        Obtén todas las listas y descarga tu Excel profesional para actuar sobre tus datos.
      </p>
      <button
        onClick={() => void onCheckout()}
        disabled={loading}
        className="rounded-xl bg-emerald-400 px-6 py-3 font-semibold text-slate-900 hover:bg-emerald-300 disabled:opacity-70"
      >
        {loading ? 'Abriendo Stripe...' : `Desbloquear por $${priceMxn} MXN`}
      </button>
    </section>
  );
}
