"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";

import { clearAuthToken, getAuthToken, onAuthChanged } from "@/lib/api";

function decodeJwtUsername(token: string): string | null {
  const parts = token.split(".");
  if (parts.length < 2) return null;

  try {
    const base64Url = parts[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
    const json = atob(padded);
    const payload = JSON.parse(json) as Record<string, unknown>;

    // Backend sets JwtRegisteredClaimNames.UniqueName => "unique_name"
    const username = payload["unique_name"];
    return typeof username === "string" && username.length > 0 ? username : null;
  } catch {
    return null;
  }
}

export default function AuthNav() {
  const router = useRouter();
  const [username, setUsername] = useState<string | null>(null);
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const isAuthed = useMemo(() => Boolean(username), [username]);

  useEffect(() => {
    const read = () => {
      const token = getAuthToken();
      setUsername(token ? decodeJwtUsername(token) : null);
      setIsMenuOpen(false);
    };

    read();
    return onAuthChanged(read);
  }, []);

  if (isAuthed) {
    return (
      <div className="navMenu">
        <button
          type="button"
          className="navLink navPill"
          aria-label="Signed in user"
          aria-haspopup="menu"
          aria-expanded={isMenuOpen}
          onClick={() => setIsMenuOpen((v) => !v)}
        >
          {username}
        </button>

        {isMenuOpen ? (
          <div className="navDropdown" role="menu">
            <button
              type="button"
              className="navDropdownItem"
              role="menuitem"
              onClick={() => {
                clearAuthToken();
                setIsMenuOpen(false);
                router.push("/login");
              }}
            >
              Log out
            </button>
          </div>
        ) : null}
      </div>
    );
  }

  return (
    <Link className="navLink navPill" href="/login">
      Sign in
    </Link>
  );
}
