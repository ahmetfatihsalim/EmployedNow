import { apiClient } from "./apiClient";
import type { AuthResponse, UserRole } from "../types/models";

export async function login(email: string, password: string): Promise<AuthResponse> {
  const response = await apiClient.post<AuthResponse>("/api/auth/login", { email, password });
  return response.data;
}

export async function register(email: string, password: string, role: UserRole): Promise<AuthResponse> {
  const response = await apiClient.post<AuthResponse>("/api/auth/register", { email, password, role });
  return response.data;
}
