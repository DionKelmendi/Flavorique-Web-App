import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import SmallRecipeItem from '../components/recipeComponents.js/SmallRecipeItem';
import CategoryItem from '../components/recipeComponents.js/CategoryItem';
import Hero from '../components/recipeComponents.js/Hero';
import TopRecipes from '../components/recipeComponents.js/TopRecipes';
import CategoryList from '../components/recipeComponents.js/CategoryList';

export default function Recipes({ userData }) {

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    return (
        <section className='recipeMainContainer'>
            <section style={{ height: "90px" }}></section>
            <Hero userData={userData} />
            <TopRecipes />
            <CategoryList amount={5} />
        </section>
    )
}