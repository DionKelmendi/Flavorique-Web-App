import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import SmallRecipeItem from './SmallRecipeItem';

export default function TopRecipes({ }) {

  const [recipeData, setRecipeData] = useState([]);
  const [error, setError] = useState([])

  useEffect(() => {
    fetch('https://localhost:7147/api/Recipe?sortOrder=rating&pageSize=9', {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        setRecipeData(data.data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  const smallRecipeItems = recipeData.map(recipe => (
    <SmallRecipeItem key={recipe.id} id={recipe.id} src={recipe.image} title={recipe.title} reviews={recipe.rating.count} rating={recipe.rating.rating} />
  ));

  return (
    <>
      <section className='topRecipes'>
        <div className='m-auto text-center w-60 pt-5'>
          <h3 className='text-primary'><i className="bi bi-trophy text-warning"></i> &nbsp; Top rated recipes &nbsp; <i className="bi bi-trophy text-warning"></i></h3>
          <p className='w-50 m-auto'>Out of all the many recipes on Flavorique, these are our shining stars - the recipes we come back to again and again (and again).</p>

          <div className='recipeContainer d-flex flex-wrap justify-content-between text-start mt-5'>
            {smallRecipeItems}
          </div>
          <div className="d-grid w-60 m-auto p-5">
            <Link to="/Recipes/All" className='btn btn-lg btn-primary'>
              <b>VIEW ALL RECIPES</b>
            </Link>
          </div>
        </div>
      </section>
    </>
  )
}
