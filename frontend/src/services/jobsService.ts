import { apiClient } from "./apiClient";
import type { JobDetail, JobSummary, PagedResult } from "../types/models";

export async function getJobs(pageNumber = 1, pageSize = 10): Promise<PagedResult<JobSummary>> {
  const response = await apiClient.get<PagedResult<JobSummary>>("/api/jobs", {
    params: { pageNumber, pageSize },
  });
  return response.data;
}

export async function getJobById(jobId: string): Promise<JobDetail> {
  const response = await apiClient.get<JobDetail>(`/api/jobs/${jobId}`);
  return response.data;
}

export async function createJob(title: string, description: string): Promise<JobSummary> {
  const response = await apiClient.post<JobSummary>("/api/jobs", { title, description });
  return response.data;
}

export async function updateJob(jobId: string, title: string, description: string): Promise<JobSummary> {
  const response = await apiClient.put<JobSummary>(`/api/jobs/${jobId}`, { title, description });
  return response.data;
}

export async function deleteJob(jobId: string): Promise<void> {
  await apiClient.delete(`/api/jobs/${jobId}`);
}
