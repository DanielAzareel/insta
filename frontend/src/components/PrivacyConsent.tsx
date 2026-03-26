interface PrivacyConsentProps {
  checked: boolean;
  onChange: (checked: boolean) => void;
}

export default function PrivacyConsent({ checked, onChange }: PrivacyConsentProps) {
  return (
    <label className="flex items-start gap-3 rounded-xl border border-slate-700 p-3 text-sm text-slate-300">
      <input
        type="checkbox"
        checked={checked}
        onChange={(e) => onChange(e.target.checked)}
        className="mt-1 h-4 w-4 rounded border-slate-500 bg-slate-800 text-cyan-400"
      />
      <span>
        Confirmo que acepto el procesamiento temporal de mis archivos. <b>Tus archivos se procesan temporalmente y no
        se almacenan en nuestros servidores.</b>
      </span>
    </label>
  );
}
