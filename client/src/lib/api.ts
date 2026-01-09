export type ApiResponse<T> = {
  message: string;
  isSuccess: boolean;
  result: T | null;
  statusCode: number;
};

export type PaginatedApiResponse<T> = ApiResponse<T> & {
  selectedPage: number;
  totalPages: number;
  pageSize: number;
  itemCount: number;
};

export class ApiError extends Error {
  public readonly status: number;
  public readonly response?: unknown;

  constructor(message: string, status: number, response?: unknown) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.response = response;
  }
}

const TOKEN_STORAGE_KEY = "baasi.jwt";

export function getAuthToken(): string | null {
  if (typeof window === "undefined") return null;
  return window.localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setAuthToken(token: string): void {
  if (typeof window === "undefined") return;
  window.localStorage.setItem(TOKEN_STORAGE_KEY, token);
}

export function clearAuthToken(): void {
  if (typeof window === "undefined") return;
  window.localStorage.removeItem(TOKEN_STORAGE_KEY);
}

export function getApiBaseUrl(): string {
  return (
    process.env.NEXT_PUBLIC_API_BASE_URL?.trim() || "https://localhost:5001"
  );
}

type ApiFetchOptions = Omit<RequestInit, "headers"> & {
  headers?: Record<string, string>;
  auth?: boolean;
};

export async function apiFetch<T>(
  path: string,
  options: ApiFetchOptions = {}
): Promise<T> {
  const url = new URL(path, getApiBaseUrl());

  const headers: Record<string, string> = {
    Accept: "application/json",
    ...options.headers,
  };

  const isJsonBody =
    typeof options.body === "string" ||
    (options.body != null && !(options.body instanceof FormData));

  if (isJsonBody && headers["Content-Type"] == null) {
    headers["Content-Type"] = "application/json";
  }

  if (options.auth) {
    const token = getAuthToken();
    if (token) headers.Authorization = `Bearer ${token}`;
  }

  const res = await fetch(url.toString(), {
    ...options,
    headers,
  });

  const contentType = res.headers.get("content-type") ?? "";
  const isJson = contentType.includes("application/json");
  const data = isJson ? await res.json().catch(() => null) : await res.text();

  if (!res.ok) {
    const message =
      (isJson && data && typeof data === "object" && "message" in data
        ? String((data as { message?: unknown }).message)
        : res.statusText) || "Request failed";
    throw new ApiError(message, res.status, data);
  }

  return data as T;
}
