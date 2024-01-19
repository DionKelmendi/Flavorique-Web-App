import { React, useState, useEffect } from 'react'
import { Link, useParams } from 'react-router-dom';
import RecipeListItem from '../components/recipeComponents.js/RecipeListItem';

export default function Category({ }) {

    const [recipeData, setRecipeData] = useState([]);
    const [categoryData, setCategoryData] = useState([]);
    const [error, setError] = useState([]);
    const { id } = useParams();

    useEffect(() => {
        if (id) {
            fetch(`https://localhost:7147/api/Category/${id}`, {
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
                    setCategoryData(data);
                    console.log(data);
                })
                .catch(error => {
                    setError(error.message);
                    console.log(error);
                });

            fetch(`https://localhost:7147/api/Recipe/category/${id}`, {
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
                    setRecipeData(data);
                    console.log(data);
                })
                .catch(error => {
                    setError(error.message);
                    console.log(error);
                });
        }
    }, [id])

    const recipeListItems = recipeData.map(recipe => (
        <RecipeListItem key={recipe.id} id={recipe.id} src={recipe.image} title={recipe.title} author={recipe.authorName} reviews={recipe.rating.count} rating={recipe.rating.rating} />
    ));

    return (
        <>
            <section>
                <section style={{ height: "90px", background: "#f7f7f7" }}></section>
                {id
                    ?
                    categoryData ?
                        <>
                            <section className="py-3">
                                <div className="m-auto text-center p-5 pb-5 w-60">
                                    <h1 className="mb-5" style={{ fontSize: "50px" }}>{categoryData.name}</h1>
                                    <h5 className='text-secondary w-75 m-auto text-justify'>Showing recipes that have atleast one tag in the category {categoryData.name} in them. Hopefully you find the recipe of your dreams!</h5>
                                </div>
                            </section>

                            <section style={{ background: "#f7f7f7" }}>
                                <div className='m-auto w-60 py-5'>
                                    <div className='d-flex flex-wrap'>
                                        {recipeListItems.length != 0 ?
                                            <>

                                                {recipeListItems}

                                            </>
                                            :
                                            <p className='text-center w-100 m-0'>No recipes found.</p>
                                        }
                                    </div>
                                </div>
                            </section>
                        </>
                        :
                        <p>Tag not found</p>
                    :
                    <p>ID was not given</p>}
            </section>
        </>
    );
}