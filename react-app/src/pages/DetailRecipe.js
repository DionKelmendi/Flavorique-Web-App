import React, { useState, useEffect, useRef } from 'react';
import { Link, useParams } from 'react-router-dom';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import RecipeItem from '../components/homeComponents.js/RecipeItem';
import SmallRecipeList from '../components/homeComponents.js/SmallRecipeList';
import CreateComent from '../components/recipeComponents.js/CreateComment';
import CommentList from '../components/recipeComponents.js/CommentList';

export default function DetailRecipe({ userData }) {
  const [recipeData, setRecipeData] = useState([]);
  const [tagData, setTagData] = useState([]);
  const [error, setError] = useState([]);
  const ingredients = useRef(null);
  const { id } = useParams();

  useEffect(() => {
    ingredients.current = document.getElementById("ingredients");

    if (ingredients.current != null) {
      let printBtn = document.createElement("a");

      let i = document.createElement("i");
      i.classList.add("bi");
      i.classList.add("bi-printer");

      printBtn.appendChild(document.createTextNode("Print Recipe "));
      printBtn.appendChild(i);
      printBtn.classList.add("btn");
      printBtn.classList.add("btn-light");
      printBtn.style.float = "right";

      printBtn.href = `../Print/${id}`;

      ingredients.current.appendChild(printBtn);
      console.log("setting recipe data");
    }
  }, [recipeData]);

  useEffect(() => {
    if (id) {
      fetch(`https://localhost:7147/api/Recipe/${id}`)
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

      fetch(`https://localhost:7147/api/Recipe/RecipeTag?recipeId=${id}`)
        .then(response => {
          if (!response.ok) {
            throw new Error(`Comments not found! Status: ${response.status}`);
          }
          return response.json();
        })
        .then(data => {
          setTagData(data);
        })
        .catch(error => {
          setError(error.message);
        });
    }
    window.scrollTo(0, 0);

  }, [id]);

  const tags = tagData.map(tag => (
    <Link key={tag.id} to={"/Tag/" + tag.tagId} className='tag-item p-2 bg-primary rounded' style={{ letterSpacing: "1px", color: "white" }}>
      {tag.tag.name}
    </Link>
  ));

  return (id ?
    <>
      {recipeData.length != 0 ?
        <>
          <div className='m-auto w-60 ck-content'>
            <div className='d-flex justify-content-between align-items-center'>
              <h1 className='py-5'>{recipeData.title}</h1>
              {
                userData
                  ?
                  userData.id == recipeData.authorId
                    ?
                    <>
                      <Link to={"/Recipes/Edit/" + recipeData.id}>
                        <button className='btn btn-warning'>Edit Recipe</button>
                      </Link>
                    </>
                    :
                    <></>
                  :
                  <></>
              }
            </div>
            <a href="#ingredients" className="w-100 d-block bg-light text-center p-3 mb-5 text-decoration-none" style={{ fontSize: "20px" }}><i className="bi bi-arrow-down"></i> Jump to recipe</a>
            <div dangerouslySetInnerHTML={{ __html: recipeData.body }} />
          </div>

          <SmallRecipeList id={id} />

          <section className="tag-container d-flex align-items-center mt-5 w-60 m-auto">
            <h4 className="me-3">Tags:</h4>
            {tags.length != 0 ?
              <>
                {tags}
              </>
              :
              <Link className='tag-item p-2 bg-primary rounded' style={{ letterSpacing: "1px", color: "white" }}>
                No Tags
              </Link>
            }
          </section>

          {
            userData
              ?
              <CreateComent recipeId={id} />
              :
              <></>
          }

          <CommentList userData={userData} recipeId={id} />
        </>
        :
        <p>Recipe does not exist</p>
      }
    </>
    :
    <>
      <p>No id given</p>
    </>
  );
}
