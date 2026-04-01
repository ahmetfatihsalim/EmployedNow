import axios from "axios";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  // Injects JWT for all authenticated requests.
  const token = localStorage.getItem("employednow_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Converts backend response envelopes into consistent UI-facing errors.
    const message = error?.response?.data?.error || error?.message || "Unexpected error";
    return Promise.reject(new Error(message));
  }
);
