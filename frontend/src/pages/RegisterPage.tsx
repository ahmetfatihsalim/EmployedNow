import { useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { register } from "../services/authService";
import type { UserRole } from "../types/models";

export function RegisterPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState<UserRole>("User");
  const [error, setError] = useState("");
  const { loginSuccess } = useAuth();
  const navigate = useNavigate();

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError("");

    try {
      const result = await register(email, password, role);
      loginSuccess(result);
      navigate("/dashboard");
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <section className="mx-auto mt-10 max-w-md rounded-2xl border border-violet-200 bg-white p-6 shadow-lg">
      <h1 className="text-2xl font-bold text-violet-900">Register</h1>
      <p className="mt-1 text-sm text-slate-500">Create your account in seconds.</p>

      <form className="mt-6 space-y-4" onSubmit={onSubmit}>
        <label className="block">
          <span className="mb-1 block text-sm font-semibold text-violet-800">Email</span>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full rounded-lg border border-violet-200 px-3 py-2 outline-none ring-violet-400 focus:ring"
            required
          />
        </label>

        <label className="block">
          <span className="mb-1 block text-sm font-semibold text-violet-800">Password</span>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full rounded-lg border border-violet-200 px-3 py-2 outline-none ring-violet-400 focus:ring"
            required
          />
        </label>

        <label className="block">
          <span className="mb-1 block text-sm font-semibold text-violet-800">Account Type</span>
          <select
            value={role}
            onChange={(e) => setRole(e.target.value as UserRole)}
            className="w-full rounded-lg border border-violet-200 px-3 py-2 outline-none ring-violet-400 focus:ring"
          >
            <option value="User">User</option>
            <option value="Company">Company</option>
          </select>
        </label>

        {error && <p className="rounded-md bg-rose-100 p-2 text-sm text-rose-700">{error}</p>}

        <button className="w-full rounded-lg bg-violet-600 py-2 font-semibold text-white hover:bg-violet-700">Register</button>
      </form>

      <p className="mt-4 text-sm text-slate-600">
        Have an account? <Link to="/login" className="font-semibold text-violet-700">Login here</Link>
      </p>
    </section>
  );
}
