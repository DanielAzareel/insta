import { Link } from 'react-router-dom';

export default function CancelPage() {
  return (
    <main className="mx-auto flex min-h-screen max-w-xl items-center px-4 py-10">
      <section className="card w-full text-center space-y-4">
        <h1 className="text-2xl font-bold">Pago cancelado</h1>
        <p className="text-slate-300">No te preocupes, tu análisis sigue disponible temporalmente para intentar de nuevo.</p>
        <Link to="/" className="inline-block rounded-lg bg-cyan-400 px-5 py-2 font-semibold text-slate-900">
          Volver al inicio
        </Link>
      </section>
    </main>
  );
}
