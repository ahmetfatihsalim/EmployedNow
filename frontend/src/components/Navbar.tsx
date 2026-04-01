import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export function Navbar() {
  const { auth, isAuthenticated, logout } = useAuth();

  return (
    <header className="border-b border-violet-200/60 bg-white/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
        <Link to="/" className="text-xl font-bold tracking-tight text-violet-800">
          EmployedNow
        </Link>

        <nav className="flex items-center gap-3 text-sm font-medium">
          <NavLink to="/" className="rounded-md px-3 py-2 text-violet-700 hover:bg-violet-100">
            Jobs
          </NavLink>

          {isAuthenticated && (
            <>
              <NavLink to="/dashboard" className="rounded-md px-3 py-2 text-violet-700 hover:bg-violet-100">
                Dashboard
              </NavLink>
              <NavLink to="/network" className="rounded-md px-3 py-2 text-violet-700 hover:bg-violet-100">
                Network
              </NavLink>
              <NavLink to="/me" className="rounded-md px-3 py-2 text-violet-700 hover:bg-violet-100">
                User Info
              </NavLink>
            </>
          )}

          {!isAuthenticated ? (
            <>
              <NavLink to="/login" className="rounded-md px-3 py-2 text-violet-700 hover:bg-violet-100">
                Login
              </NavLink>
              <NavLink to="/register" className="rounded-md bg-violet-600 px-3 py-2 text-white hover:bg-violet-700">
                Register
              </NavLink>
            </>
          ) : (
            <button onClick={logout} className="rounded-md bg-violet-600 px-3 py-2 text-white hover:bg-violet-700">
              Logout ({auth?.role})
            </button>
          )}
        </nav>
      </div>
    </header>
  );
}
