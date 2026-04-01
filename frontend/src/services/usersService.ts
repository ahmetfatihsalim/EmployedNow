import { apiClient } from "./apiClient";
import type { DiscoverUser, PagedResult } from "../types/models";

export async function getUsers(pageNumber = 1, pageSize = 20, role?: "User" | "Company") {
  const response = await apiClient.get<PagedResult<DiscoverUser>>("/api/users", {
    params: { pageNumber, pageSize, role },
  });

  return response.data;
}
