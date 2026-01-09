"use client";

import { useRouter } from "next/navigation";
import { useMemo, useState } from "react";

import { apiFetch, ApiError, type ApiResponse, setAuthToken } from "@/lib/api";

type LoginResult = {
  id: number;
  username: string;
  token: string;
};

type AuthMode = "login" | "register";

type RegisterPayload = {
  name: string;
  surname: string;
  username: string;
  email: string;
  password: string;
};

export default function LoginPage() {
  const router = useRouter();
  const [mode, setMode] = useState<AuthMode>("login");
  const [registerForm, setRegisterForm] = useState<RegisterPayload>({
    name: "",
    surname: "",
    username: "",
    email: "",
    password: "",
  });
  const [loginUsername, setLoginUsername] = useState("");
  const [loginPassword, setLoginPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = useMemo(() => {
    if (isSubmitting) return false;

    if (mode === "login") {
      return loginUsername.trim().length >= 3 && loginPassword.length >= 1;
    }

    return (
      registerForm.name.trim().length >= 2 &&
      registerForm.surname.trim().length >= 2 &&
      registerForm.username.trim().length >= 3 &&
      registerForm.email.trim().length >= 3 &&
      registerForm.password.length >= 8
    );
  }, [mode, loginUsername, loginPassword, registerForm, isSubmitting]);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      if (mode === "register") {
        const registerRes = await apiFetch<ApiResponse<LoginResult>>("/api/account/register", {
          method: "POST",
          body: JSON.stringify({
            name: registerForm.name.trim(),
            surname: registerForm.surname.trim(),
            username: registerForm.username.trim(),
            email: registerForm.email.trim(),
            password: registerForm.password,
          }),
        });

        if (!registerRes.isSuccess || !registerRes.result?.token) {
          setError(registerRes.message || "Registration failed");
          return;
        }

        setAuthToken(registerRes.result.token);
        router.push("/");
        return;
      }

      const res = await apiFetch<ApiResponse<LoginResult>>("/api/account/login", {
        method: "POST",
        body: JSON.stringify({
          username: loginUsername.trim(),
          password: loginPassword,
        }),
      });

      if (!res.isSuccess || !res.result?.token) {
        setError(res.message || "Login failed");
        return;
      }

      setAuthToken(res.result.token);
      router.push("/");
    } catch (err) {
      if (err instanceof ApiError) setError(err.message);
      else setError("Something went wrong. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="pageContainer">
      <div className="card authCard">
        <h1 className="cardTitle">{mode === "login" ? "Sign in" : "Create account"}</h1>
        <p className="cardSubtitle">
          {mode === "login" ? "Welcome back to Baasi." : "Sign up inside Baasi."}
        </p>

        <div className="authMode">
          <button
            type="button"
            className={mode === "login" ? "authModeBtn authModeBtnActive" : "authModeBtn"}
            onClick={() => {
              setError(null);
              setMode("login");
            }}
          >
            Sign in
          </button>
          <button
            type="button"
            className={mode === "register" ? "authModeBtn authModeBtnActive" : "authModeBtn"}
            onClick={() => {
              setError(null);
              setMode("register");
            }}
          >
            Sign up
          </button>
        </div>

        <form className="form" onSubmit={onSubmit}>
          {mode === "register" ? (
            <>
              <div className="formRow">
                <label className="field">
                  <span className="label">First name</span>
                  <input
                    className="input"
                    value={registerForm.name}
                    onChange={(e) =>
                      setRegisterForm((s) => ({ ...s, name: e.target.value }))
                    }
                    autoComplete="given-name"
                    required
                  />
                </label>

                <label className="field">
                  <span className="label">Last name</span>
                  <input
                    className="input"
                    value={registerForm.surname}
                    onChange={(e) =>
                      setRegisterForm((s) => ({ ...s, surname: e.target.value }))
                    }
                    autoComplete="family-name"
                    required
                  />
                </label>
              </div>

              <label className="field">
                <span className="label">Username</span>
                <input
                  className="input"
                  value={registerForm.username}
                  onChange={(e) =>
                    setRegisterForm((s) => ({ ...s, username: e.target.value }))
                  }
                  autoComplete="username"
                  required
                />
              </label>

              <label className="field">
                <span className="label">Email</span>
                <input
                  className="input"
                  type="email"
                  value={registerForm.email}
                  onChange={(e) =>
                    setRegisterForm((s) => ({ ...s, email: e.target.value }))
                  }
                  autoComplete="email"
                  required
                />
              </label>

              <label className="field">
                <span className="label">Password</span>
                <input
                  className="input"
                  type="password"
                  value={registerForm.password}
                  onChange={(e) =>
                    setRegisterForm((s) => ({ ...s, password: e.target.value }))
                  }
                  autoComplete="new-password"
                  required
                />
              </label>
            </>
          ) : (
            <>
              <label className="field">
                <span className="label">Username</span>
                <input
                  className="input"
                  value={loginUsername}
                  onChange={(e) => setLoginUsername(e.target.value)}
                  autoComplete="username"
                  required
                />
              </label>

              <label className="field">
                <span className="label">Password</span>
                <input
                  className="input"
                  type="password"
                  value={loginPassword}
                  onChange={(e) => setLoginPassword(e.target.value)}
                  autoComplete="current-password"
                  required
                />
              </label>
            </>
          )}

          {error ? <div className="formError">{error}</div> : null}

          <button className="button" type="submit" disabled={!canSubmit}>
            {isSubmitting
              ? mode === "login"
                ? "Signing in…"
                : "Creating account…"
              : mode === "login"
                ? "Sign in"
                : "Create account"}
          </button>
        </form>
      </div>
    </main>
  );
}
