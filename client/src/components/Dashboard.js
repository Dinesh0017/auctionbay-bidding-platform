import React from "react";
import DashboardMenu from "./DashboardMenu";
import DashboardHeader from "./DashboardHeader";
function Dashboard({ children, links, name }) {
  return (
    <div className="w-full flex min:h-screen">
      <DashboardMenu links={links} name={name} />
      <div className="w-full lg:w-[80%] h-full">
        <DashboardHeader links={links} />
        <div className="w-full h-full bg-zinc-950  overflow-x-hidden p-5">
          {children}
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
