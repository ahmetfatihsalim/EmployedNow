import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getMe, updatePremium, type UserProfile } from "../services/usersMeService";

export function UserInfoPage() {
  const { auth, loginSuccess } = useAuth();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [message, setMessage] = useState("");

  useEffect(() => {
    getMe()
      .then(setProfile)
      .catch((err: Error) => setMessage(err.message));
  }, []);

  async function onUpgradePremium() {
    try {
      const updated = await updatePremium(true);
      setProfile(updated);
      setMessage("Premium status updated.");

      if (auth) {
        // Keeps navbar/session state aligned after profile premium change.
        loginSuccess({
          token: auth.token,
          userId: auth.userId,
          email: auth.email,
          role: auth.role,
          isPremium: updated.isPremium,
        });
      }
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  if (!profile) {
    return <section className="mx-auto max-w-4xl px-4 py-10 text-slate-600">Loading user information...</section>;
  }

  return (
    <section className="mx-auto max-w-4xl px-4 py-8">
      <div className="rounded-2xl border border-violet-200 bg-white p-6 shadow-md">
        <h1 className="text-2xl font-bold text-violet-900">User Information</h1>
        <p className="mt-4 text-slate-700"><strong>Email:</strong> {profile.email}</p>
        <p className="mt-2 text-slate-700"><strong>Role:</strong> {profile.role}</p>
        <p className="mt-2 text-slate-700"><strong>Premium:</strong> {profile.isPremium ? "Yes" : "No"}</p>
        <p className="mt-2 text-slate-700"><strong>Joined:</strong> {new Date(profile.createdAt).toLocaleDateString()}</p>

        {!profile.isPremium && (
          <button onClick={onUpgradePremium} className="mt-6 rounded-lg bg-violet-600 px-4 py-2 font-semibold text-white hover:bg-violet-700">
            Upgrade to Premium
          </button>
        )}

        {message && <p className="mt-4 rounded-lg bg-violet-100 p-3 text-sm text-violet-800">{message}</p>}
      </div>
    </section>
  );
}
