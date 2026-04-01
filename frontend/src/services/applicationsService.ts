import { apiClient } from "./apiClient";
import type { JobApplication } from "../types/models";

export async function applyToJob(jobId: string): Promise<JobApplication> {
  const response = await apiClient.post<JobApplication>("/api/applications", { jobId });
  return response.data;
}

export async function getApplicationsForJob(jobId: string): Promise<JobApplication[]> {
  const response = await apiClient.get<JobApplication[]>(`/api/applications/job/${jobId}`);
  return response.data;
}
