import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import type { UserRole } from "../types/models";

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: UserRole[];
}

export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { isAuthenticated, auth } = useAuth();

  // Unauthenticated users are always sent to login.
  if (!isAuthenticated || !auth) {
    return <Navigate to="/login" replace />;
  }

  // Authenticated users without permission are routed to their dashboard.
  if (allowedRoles && !allowedRoles.includes(auth.role)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
}
