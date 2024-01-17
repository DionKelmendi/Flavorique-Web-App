import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import RecipeItem from './RecipeItem';

export default function SmallRecipeList({ id }) {

  const [recipeData, setRecipeData] = useState([]);
  const [error, setError] = useState([]);

  useEffect(() => {
    fetch("https://localhost:7147/api/Recipe/short?number=3")
      .then(response => {
        if (!response.ok) {
          throw new Error(`Recipe not found! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        setRecipeData(data);
      })
      .catch(error => {
        setError(error.message);
      });
  }, [])

  const recipeItems = recipeData.map(recipe => (
    <RecipeItem
      key={recipe.id}
      id={recipe.id}
      date={new Date(recipe.createdDateTime).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      })}
      title={recipe.title}
      description={recipe.body}
      src={recipe.image}
    />
  ));

  return (
    <section className='recipeList'>
      <div className='m-auto pb-4 w-60'>
        {id ?
          <h3 className='mt-5 text-primary mb-5'>Similar Recipes</h3>
          :
          <h3 className='mt-5 text-primary mb-5'>Our latest recipes</h3>
        }
        {recipeItems}
        {id ?
          <></>
          :
          <div className='text-center'>
            <Link to="/Recipes/All?sort=recent" className='btn btn-lg btn-primary'>VIEW MORE RECENT POSTS</Link>
          </div>
        }
      </div>
    </section>
  )
}