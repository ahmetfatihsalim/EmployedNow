import { createContext, useContext, useMemo, useState } from "react";
import type { AuthResponse, UserRole } from "../types/models";

interface AuthState {
  token: string;
  userId: string;
  email: string;
  role: UserRole;
  isPremium: boolean;
}

interface AuthContextValue {
  auth: AuthState | null;
  isAuthenticated: boolean;
  loginSuccess: (payload: AuthResponse) => void;
  logout: () => void;
}

const AUTH_STORAGE_KEY = "employednow_auth";

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function readStoredAuth(): AuthState | null {
  const raw = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!raw) {
    return null;
  }

  try {
    const parsed = JSON.parse(raw) as Partial<AuthState> & { role?: UserRole | number | string };
    if (!parsed.token || !parsed.userId || !parsed.email) {
      return null;
    }

    // Accepts both legacy numeric roles and current string roles from storage.
    return {
      token: parsed.token,
      userId: parsed.userId,
      email: parsed.email,
      role: normalizeRole(parsed.role),
      isPremium: Boolean(parsed.isPremium),
    };
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

function normalizeRole(input: UserRole | number | string | undefined): UserRole {
  // Keeps older persisted sessions compatible after API enum serialization changes.
  if (input === "Company" || input === 2 || input === "2") {
    return "Company";
  }

  return "User";
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(() => readStoredAuth());

  const value = useMemo<AuthContextValue>(() => ({
    auth,
    isAuthenticated: Boolean(auth?.token),
    loginSuccess: (payload) => {
      const nextState: AuthState = {
        token: payload.token,
        userId: payload.userId,
        email: payload.email,
        role: normalizeRole(payload.role),
        isPremium: payload.isPremium,
      };

      // Stores both full auth state and raw token for API interceptor usage.
      localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(nextState));
      localStorage.setItem("employednow_token", payload.token);
      setAuth(nextState);
    },
    logout: () => {
      localStorage.removeItem(AUTH_STORAGE_KEY);
      localStorage.removeItem("employednow_token");
      setAuth(null);
    },
  }), [auth]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider");
  }

  return context;
}
