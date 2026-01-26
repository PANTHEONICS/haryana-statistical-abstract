import TopNav from "./TopNav"
import DepartmentHeader from "./DepartmentHeader"

export default function AppLayout({ children }) {
  return (
    <div className="min-h-screen bg-background">
      <DepartmentHeader />
      <TopNav />
      <main className="w-full">
        {children}
      </main>
    </div>
  )
}
