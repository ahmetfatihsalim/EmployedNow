export type UserRole = "User" | "Company";

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  role: UserRole;
  isPremium: boolean;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
}

export interface JobSummary {
  id: string;
  title: string;
  description: string;
  companyId: string;
  createdAt: string;
}

export interface JobDetail extends JobSummary {
  companyEmail: string;
}

export interface JobApplication {
  id: string;
  jobPostingId: string;
  userId: string;
  userEmail: string;
  appliedAt: string;
}

export interface ConnectionItem {
  id: string;
  requesterId: string;
  requesterEmail: string;
  targetId: string;
  targetEmail: string;
  status: "Pending" | "Accepted";
  createdAt: string;
}

export interface DiscoverUser {
  id: string;
  email: string;
  role: UserRole;
  isPremium: boolean;
  createdAt: string;
}
