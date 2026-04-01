import { apiClient } from "./apiClient";
import type { ConnectionItem } from "../types/models";

export async function getConnections(): Promise<ConnectionItem[]> {
  const response = await apiClient.get<ConnectionItem[]>("/api/connections");
  return response.data;
}

export async function sendConnectionRequest(targetUserId: string): Promise<ConnectionItem> {
  const response = await apiClient.post<ConnectionItem>("/api/connections", { targetUserId });
  return response.data;
}

export async function acceptConnectionRequest(requestId: string): Promise<ConnectionItem> {
  const response = await apiClient.put<ConnectionItem>(`/api/connections/${requestId}`);
  return response.data;
}
