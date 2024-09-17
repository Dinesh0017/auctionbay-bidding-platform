"use client";

import Categories from "@/components/Categories";
import Hero from "@/components/Hero";
import ShowCase from "@/components/ShowCase";

export default function Home() {
  return (
    <>
    <main className="flex flex-col gap-5">
      <Hero/>
      <Categories/>
      <ShowCase />
      <ShowCase />
    </main>
    </>
  );
}
