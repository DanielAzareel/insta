interface MetricsCardsProps {
  followersCount: number;
  followingCount: number;
  mutualCount: number;
  notFollowingBackCount: number;
  fansCount: number;
}

const metrics = [
  { key: 'followersCount', label: 'Followers' },
  { key: 'followingCount', label: 'Following' },
  { key: 'mutualCount', label: 'Mutuos' },
  { key: 'notFollowingBackCount', label: 'No te siguen' },
  { key: 'fansCount', label: 'Fans' },
] as const;

export default function MetricsCards(props: MetricsCardsProps) {
  return (
    <section className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
      {metrics.map((metric) => (
        <article key={metric.key} className="card text-center">
          <p className="text-sm text-slate-400">{metric.label}</p>
          <p className="text-3xl font-bold text-cyan-300">{props[metric.key]}</p>
        </article>
      ))}
    </section>
  );
}
