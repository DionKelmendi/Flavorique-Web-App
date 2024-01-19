import { React, useState, useEffect } from 'react'

export default function CreateComent({ recipeId }) {

  const [commentData, setCommentData] = useState("");
  const [ratingData, setRatingData] = useState("");
  const [error, setError] = useState("");

  const handleCommentValue = (e) => {
    setCommentData(e.target.value);
  }

  const handleRatingValue = (e) => {
    setRatingData(e.target.value);
  }

  const handleFormSubmit = (e) => {
    e.preventDefault();

    fetch(`https://localhost:7147/api/Comment/PostComment?Body=${commentData}&Rating=${ratingData}&RecipeId=${recipeId}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log(data);

        window.location.reload();
      })
      .catch(error => {
        console.log(error.message);
        setError(error.message);
      });
  };

  return (
    <>
      <div className='m-auto w-60'>
        <section className="create-comment-container mt-5 p-5 bg-light rounded">
          <h4 className="mb-5">Leave a comment</h4>
          <form onSubmit={handleFormSubmit} method="post">
            <div className="mb-3">
              <label>Comment</label>
              <textarea onChange={handleCommentValue} style={{ minHeight: "200px" }} className="form-control"></textarea>
              <span className="text-danger"></span>
            </div>
            <div className="mb-3 star-container">
              <label className="me-3">Recipe rating</label>
              <fieldset onChange={handleRatingValue} className="star-rating">
                <input id="star-1" type="radio" name="rating" value="1" />
                <label htmlFor="star-1">
                  <i className="bi bi-star-fill"></i>
                </label>
                <input id="star-2" type="radio" name="rating" value="2" />
                <label htmlFor="star-2">
                  <i className="bi bi-star-fill"></i>
                </label>
                <input id="star-3" type="radio" name="rating" value="3" />
                <label htmlFor="star-3">
                  <i className="bi bi-star-fill"></i>
                </label>
                <input id="star-4" type="radio" name="rating" value="4" />
                <label htmlFor="star-4">
                  <i className="bi bi-star-fill"></i>
                </label>
                <input id="star-5" type="radio" name="rating" value="5" />
                <label htmlFor="star-5">
                  <i className="bi bi-star-fill"></i>
                </label>
              </fieldset>
              <span className="text-danger"></span>
            </div>
            <button type="submit" style={{ letterSpacing: "1px" }} className="p-3 mt-4 btn btn-primary">Post Comment</button>
          </form>

        </section>
      </div>
    </>
  )
}