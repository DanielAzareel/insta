import { useState } from 'react';
import Hero from '../components/Hero';
import UploadForm from '../components/UploadForm';
import MetricsCards from '../components/MetricsCards';
import PreviewList from '../components/PreviewList';
import PaywallCard from '../components/PaywallCard';
import ErrorState from '../components/ErrorState';
import { createCheckout, uploadAnalysis } from '../api/analysisApi';
import type { AnalysisPreviewResponse } from '../types/api';

export default function HomePage() {
  const [preview, setPreview] = useState<AnalysisPreviewResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleUpload = async (followersFile: File, followingFile: File) => {
    try {
      setLoading(true);
      setError(null);
      const result = await uploadAnalysis(followersFile, followingFile);
      setPreview(result);
    } catch {
      setError('No pudimos analizar tus archivos. Verifica que sean exportaciones válidas de Instagram.');
    } finally {
      setLoading(false);
    }
  };

  const handleCheckout = async () => {
    if (!preview) return;
    try {
      setCheckoutLoading(true);
      const { checkoutUrl } = await createCheckout(preview.analysisToken);
      window.location.href = checkoutUrl;
    } catch {
      setError('No fue posible iniciar el pago. Intenta de nuevo en unos segundos.');
    } finally {
      setCheckoutLoading(false);
    }
  };

  return (
    <main className="mx-auto max-w-6xl space-y-8 px-4 py-8 md:py-12">
      <Hero />
      <UploadForm loading={loading} onSubmit={handleUpload} />
      {error && <ErrorState message={error} />}

      {preview && (
        <section className="space-y-6">
          <MetricsCards
            followersCount={preview.followersCount}
            followingCount={preview.followingCount}
            mutualCount={preview.mutualCount}
            notFollowingBackCount={preview.notFollowingBackCount}
            fansCount={preview.fansCount}
          />

          <div className="grid gap-4 lg:grid-cols-3">
            <PreviewList
              title="No te siguen (Top 10)"
              users={preview.previewNotFollowingBack}
              lockedCount={preview.lockedNotFollowingBackCount}
            />
            <PreviewList title="Fans (Top 10)" users={preview.previewFans} lockedCount={preview.lockedFansCount} />
            <PreviewList title="Mutuos (Top 10)" users={preview.previewMutuals} lockedCount={preview.lockedMutualsCount} />
          </div>

          <PaywallCard priceMxn={preview.priceMxn} loading={checkoutLoading} onCheckout={handleCheckout} />
        </section>
      )}
    </main>
  );
}
