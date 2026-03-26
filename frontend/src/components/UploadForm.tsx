import { useState } from 'react';
import PrivacyConsent from './PrivacyConsent';

interface UploadFormProps {
  loading: boolean;
  onSubmit: (followersFile: File, followingFile: File) => Promise<void>;
}

export default function UploadForm({ loading, onSubmit }: UploadFormProps) {
  const [followersFile, setFollowersFile] = useState<File | null>(null);
  const [followingFile, setFollowingFile] = useState<File | null>(null);
  const [consent, setConsent] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!followersFile || !followingFile) {
      setError('Debes subir followers_1.json y following.json');
      return;
    }
    if (!consent) {
      setError('Debes aceptar el consentimiento de privacidad para analizar tus archivos.');
      return;
    }

    setError(null);
    await onSubmit(followersFile, followingFile);
  };

  return (
    <form className="card space-y-4" onSubmit={handleSubmit}>
      <h2 className="text-xl font-semibold">Sube tus archivos exportados</h2>
      <div className="grid gap-4 md:grid-cols-2">
        <FileInput label="followers_1.json" onFile={(file) => setFollowersFile(file)} />
        <FileInput label="following.json" onFile={(file) => setFollowingFile(file)} />
      </div>
      <PrivacyConsent checked={consent} onChange={setConsent} />
      {error && <p className="text-sm text-rose-300">{error}</p>}
      <button
        type="submit"
        disabled={loading}
        className="w-full rounded-xl bg-cyan-400 px-5 py-3 font-semibold text-slate-900 transition hover:bg-cyan-300 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {loading ? 'Analizando archivos...' : 'Analizar ahora'}
      </button>
    </form>
  );
}

function FileInput({ label, onFile }: { label: string; onFile: (file: File | null) => void }) {
  return (
    <label className="space-y-2 text-sm text-slate-300">
      <span className="font-medium">{label}</span>
      <input
        type="file"
        accept=".json"
        onChange={(e) => onFile(e.target.files?.[0] ?? null)}
        className="block w-full rounded-lg border border-slate-700 bg-slate-900 p-2"
      />
    </label>
  );
}
