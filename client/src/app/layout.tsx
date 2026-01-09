import type { Metadata } from "next";
import Link from "next/link";
import { Space_Grotesk } from "next/font/google";
import "./globals.css";

const primaryFont = Space_Grotesk({
  variable: "--font-primary",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Baasi",
  description: "Baasi",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={primaryFont.variable}>
        <div className="appShell">
          <header className="nav">
            <div className="navInner">
              <Link className="navLink brand" href="/">
                Ba<span className="brandAccent">asi</span>
              </Link>
              <Link className="navLink navPill" href="/login">
                Sign in
              </Link>
            </div>
          </header>
          {children}
        </div>
      </body>
    </html>
  );
}
