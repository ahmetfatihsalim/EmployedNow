import { apiClient } from "./apiClient";
import type { UserRole } from "../types/models";

export interface UserProfile {
  id: string;
  email: string;
  role: UserRole;
  isPremium: boolean;
  createdAt: string;
}

export async function getMe(): Promise<UserProfile> {
  const response = await apiClient.get<UserProfile>("/api/users/me");
  return response.data;
}

export async function updatePremium(isPremium: boolean): Promise<UserProfile> {
  const response = await apiClient.put<UserProfile>("/api/users/me/premium", { isPremium });
  return response.data;
}
