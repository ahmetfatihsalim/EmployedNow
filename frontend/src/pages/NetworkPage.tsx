import { useEffect, useMemo, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { acceptConnectionRequest, getConnections, sendConnectionRequest } from "../services/connectionsService";
import { getUsers } from "../services/usersService";
import type { ConnectionItem, DiscoverUser } from "../types/models";

export function NetworkPage() {
  const { auth } = useAuth();
  const [users, setUsers] = useState<DiscoverUser[]>([]);
  const [connections, setConnections] = useState<ConnectionItem[]>([]);
  const [message, setMessage] = useState("");

  useEffect(() => {
    Promise.all([getUsers(1, 50), getConnections()])
      .then(([usersResult, connectionsResult]) => {
        setUsers(usersResult.items);
        setConnections(connectionsResult);
      })
      .catch((err: Error) => setMessage(err.message));
  }, []);

  const pendingIncoming = useMemo(
    () => connections.filter((x) => x.status === "Pending" && x.targetId === auth?.userId),
    [connections, auth?.userId]
  );

  const existingTargets = useMemo(() => {
    // Prevents offering Connect button when a relation already exists in either direction.
    const ids = new Set<string>();
    for (const connection of connections) {
      if (connection.requesterId === auth?.userId) {
        ids.add(connection.targetId);
      }
      if (connection.targetId === auth?.userId) {
        ids.add(connection.requesterId);
      }
    }
    return ids;
  }, [connections, auth?.userId]);

  async function onSendRequest(targetUserId: string) {
    try {
      const result = await sendConnectionRequest(targetUserId);
      setConnections((prev) => [result, ...prev]);
      setMessage("Connection request sent.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  async function onAccept(requestId: string) {
    try {
      const updated = await acceptConnectionRequest(requestId);
      // Updates just the accepted row instead of refetching all data.
      setConnections((prev) => prev.map((c) => (c.id === updated.id ? updated : c)));
      setMessage("Connection accepted.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  return (
    <section className="mx-auto max-w-6xl px-4 py-8">
      <h1 className="text-3xl font-bold text-violet-900">Network</h1>
      <p className="mt-2 text-slate-600">Send requests and grow your professional connections.</p>

      {message && <p className="mt-4 rounded-md bg-violet-100 p-3 text-sm text-violet-800">{message}</p>}

      <div className="mt-6 grid gap-6 lg:grid-cols-2">
        <div className="rounded-2xl border border-violet-200 bg-white p-5 shadow-sm">
          <h2 className="text-xl font-semibold text-violet-900">Discover Users</h2>
          <ul className="mt-3 space-y-2">
            {users
              .filter((u) => u.id !== auth?.userId)
              .map((user) => (
                <li key={user.id} className="flex items-center justify-between rounded-md bg-violet-50 p-2 text-sm">
                  <span>{user.email} ({user.role})</span>
                  {existingTargets.has(user.id) ? (
                    <span className="text-xs text-slate-500">Connected or pending</span>
                  ) : (
                    <button onClick={() => onSendRequest(user.id)} className="rounded-md bg-violet-600 px-3 py-1 text-white">
                      Connect
                    </button>
                  )}
                </li>
              ))}
          </ul>
        </div>

        <div className="rounded-2xl border border-violet-200 bg-white p-5 shadow-sm">
          <h2 className="text-xl font-semibold text-violet-900">Incoming Requests</h2>
          <ul className="mt-3 space-y-2">
            {pendingIncoming.map((connection) => (
              <li key={connection.id} className="flex items-center justify-between rounded-md bg-violet-50 p-2 text-sm">
                <span>{connection.requesterEmail}</span>
                <button onClick={() => onAccept(connection.id)} className="rounded-md bg-violet-600 px-3 py-1 text-white">
                  Accept
                </button>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </section>
  );
}
