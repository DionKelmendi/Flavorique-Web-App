import { React, useState } from 'react'
import { Link } from 'react-router-dom';
import { useEffect } from 'react';
import RecipeItem from '../components/homeComponents.js/RecipeItem';
import Hero from '../components/homeComponents.js/Hero';
import SmallRecipeList from '../components/homeComponents.js/SmallRecipeList';
import Follow from '../components/homeComponents.js/Follow';

export default function Home() {

  return (
    <section className='homeContainer'>
      <section style={{ height: "90px" }}>
      </section>
      <Hero />
      <SmallRecipeList />
      <Follow />
    </section>
  )
}
