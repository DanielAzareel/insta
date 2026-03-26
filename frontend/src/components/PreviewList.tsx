interface PreviewListProps {
  title: string;
  users: string[];
  lockedCount: number;
}

export default function PreviewList({ title, users, lockedCount }: PreviewListProps) {
  return (
    <article className="card">
      <h3 className="text-lg font-semibold">{title}</h3>
      <ul className="mt-4 space-y-2 text-sm text-slate-200">
        {users.map((user) => (
          <li key={user} className="rounded-lg border border-slate-800 p-2">@{user}</li>
        ))}
      </ul>
      {lockedCount > 0 && (
        <p className="mt-4 rounded-lg border border-amber-500/30 bg-amber-400/10 p-3 text-sm text-amber-200">
          +{lockedCount} resultados bloqueados en esta lista.
        </p>
      )}
    </article>
  );
}
