import type { Metadata } from "next";
import Link from "next/link";
import { Space_Grotesk } from "next/font/google";
import "./globals.css";

const primaryFont = Space_Grotesk({
  variable: "--font-primary",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "MyForum",
  description: "MyForum",
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
              <Link className="navLink navPill" href="/login">
                Sign in
              </Link>
              <Link className="navLink brand" href="/">
                My<span className="brandAccent">Forum</span>
              </Link>
            </div>
          </header>
          {children}
        </div>
      </body>
    </html>
  );
}
