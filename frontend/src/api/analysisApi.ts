import { api } from './client';
import type {
  AnalysisPreviewResponse,
  CheckoutResponse,
  FullReportResponse,
  PaymentStatusResponse,
} from '../types/api';

export async function uploadAnalysis(followersFile: File, followingFile: File) {
  const formData = new FormData();
  formData.append('followersFile', followersFile);
  formData.append('followingFile', followingFile);

  const { data } = await api.post<AnalysisPreviewResponse>('/api/analysis/preview', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return data;
}

export async function createCheckout(analysisToken: string) {
  const { data } = await api.post<CheckoutResponse>('/api/payments/checkout', { analysisToken });
  return data;
}

export async function getPaymentStatus(analysisToken: string) {
  const { data } = await api.get<PaymentStatusResponse>(`/api/payments/status/${analysisToken}`);
  return data;
}

export async function getFullReport(analysisToken: string) {
  const { data } = await api.get<FullReportResponse>(`/api/analysis/full/${analysisToken}`);
  return data;
}

export function getExportUrl(analysisToken: string) {
  return `${api.defaults.baseURL}/api/analysis/export/${analysisToken}`;
}
