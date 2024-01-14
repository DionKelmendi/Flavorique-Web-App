import { React, useState, useEffect } from 'react'
import RecipeListItem from '../components/recipeComponents.js/RecipeListItem';
import { Link } from 'react-router-dom';
import CategoryList from '../components/recipeComponents.js/CategoryList';
import RecipeList from '../components/recipeComponents.js/RecipeList';

export default function AllRecipes({ }) {

  return (
    <>
      <section className='recipeListContainer'>
        <RecipeList />
        <CategoryList amount={3} />
      </section>
    </>
  )
}