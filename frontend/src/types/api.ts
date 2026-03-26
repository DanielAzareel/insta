export interface AnalysisPreviewResponse {
  analysisToken: string;
  followersCount: number;
  followingCount: number;
  mutualCount: number;
  notFollowingBackCount: number;
  fansCount: number;
  previewNotFollowingBack: string[];
  previewFans: string[];
  previewMutuals: string[];
  lockedNotFollowingBackCount: number;
  lockedFansCount: number;
  lockedMutualsCount: number;
  priceMxn: number;
  expiresAt: string;
}

export interface CheckoutResponse {
  checkoutUrl: string;
}

export interface PaymentStatusResponse {
  paid: boolean;
  expiresAt?: string;
}

export interface FullReportResponse {
  analysisToken: string;
  notFollowingBack: string[];
  fans: string[];
  mutuals: string[];
}
