export default function LoadingState({ text = 'Cargando...' }: { text?: string }) {
  return <p className="card text-center text-slate-300">{text}</p>;
}
