import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import SmallRecipeItem from '../components/recipeComponents.js/SmallRecipeItem';
import CommentItem from '../components/dashboardComponents/CommentItem';

export default function Dashboard({ userData }) {

  const [error, setError] = useState({})
  const [passwordError, setPasswordError] = useState([])
  const [recipeData, setRecipeData] = useState([])
  const [commentData, setCommentData] = useState([])
  const [oldPassword, setOldPassword] = useState([])
  const [newPassword, setNewPassword] = useState([])
  const [confirmPassword, setConfirmPassword] = useState([])

  useEffect(() => {
    if (userData) {

      fetch(`https://localhost:7147/api/Recipe/user/${userData.userName}`, {
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
        })
        .catch(error => {
          setError(error.message);
          console.log(error);
        });

      fetch(`https://localhost:7147/api/Comment/GetCommentsByUser/${userData.id}`, {
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
          setCommentData(data);
        })
        .catch(error => {
          setError(error.message);
          console.log(error);
        });
    }

  }, []);

  const smallRecipeItems = recipeData.map(recipe => (
    <SmallRecipeItem
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

  const commentItems = commentData.map(comment => (
    <CommentItem
      key={comment.id}
      id={comment.id}
      body={comment.body}
      date={new Date(comment.createdDateTime).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      })}
      rating={5}
    />
  ));

  const handlePasswordChange = (e) => {
    e.preventDefault();

    const jsonData = {
      "oldPassword": oldPassword,
      "newPassword": newPassword,
      "confirmPassword": confirmPassword,
    };

    fetch(`https://localhost:7147/api/Account/change-password/${userData.id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(jsonData),
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
      })
      .catch(error => {
        console.log(error.message);
        setError(error.message);
      });

  }

  const handleOldPasswordValue = (e) => {
    setOldPassword(e.target.value);
  }
  const handleNewPasswordValue = (e) => {
    setNewPassword(e.target.value);
  }
  const handleConfirmPasswordValue = (e) => {
    setConfirmPassword(e.target.value);
  }

  return (
    <>
      {userData ?

        <section className="dashboard">
          <div className='containerMain'>
            <div className='userWelcome item'>
              <img src={"https://localhost:7147/logo"}></img>
              <p className='welcomeMessage'>Welcome back, <b>{userData.userName}</b>!</p>
              <button type='submit'>Logout</button>
            </div>
            <div className='userInfo item'>
              <p>Username <span>{userData.userName}</span></p>
              <hr />

              <p>Email <span>{userData.email}</span></p>
              <hr />

              <p>Phone <span>{userData.phoneNumber}</span></p>
              <hr />

              <button>Edit Settings</button>
            </div>

            <div className="settingsForm d-none">
              <button className="close">x</button>

              <form method="post" id='registration-form'>

                <input type='hidden' name="id" defaultValue={userData.id} />

                <div>
                  <label htmlFor="username"> Username </label><br />
                  <input name="username" type="text" defaultValue={userData.userName} /> <br />
                </div>


                <hr />

                <div>
                  <label htmlFor="email"> Email </label><br />
                  <input name="email" type="text" defaultValue={userData.email} />
                </div>

                <hr />

                <div>
                  <label htmlFor="phone"> Phone </label><br />
                  <input name="phone" type="text" defaultValue={userData.phoneNumber} />
                </div>

                <hr />

                <button className="">Edit Settings</button>

              </form>
            </div>

            <section className='changePassword item'>
              <div className='container'>
                <h1>Change Password</h1>
                <form method="post" noValidate id="password-change-form" onSubmit={handlePasswordChange}>
                  <div>
                    <label htmlFor='password'>Old Password</label>
                    <input onChange={handleOldPasswordValue} required name="password" label="Old Password" type="password" id="password" />
                    <a><i id="togglePassword" className="bi bi-eye-slash-fill"></i></a>
                  </div>
                  <div>
                    <label htmlFor='password'>New Password</label>
                    <input onChange={handleNewPasswordValue} required name="password" label="New Password" type="password" id="password" />
                    <a><i id="togglePassword" className="bi bi-eye-slash-fill"></i></a>
                  </div>
                  <div>
                    <label htmlFor='password2'>Confirm Password</label>
                    <input onChange={handleConfirmPasswordValue} required name="password2" label="Confirm New Password" type="password" id="password2" />
                    <a><i id="togglePassword2" className="bi bi-eye-slash-fill"></i></a>
                  </div>
                  <button className="update mb-5" type="submit"> Update </button>
                  <span> {passwordError} </span>
                </form>
              </div>
            </section>

            <div className='userFavorites item'>
              <h1>Most Recent Recipes</h1>
              {smallRecipeItems.length > 0 ? smallRecipeItems : <p>No recipes found.</p>}
              <Link className="mb-3" to={"Recipes/" + userData.id}>
                <button>See all Recipes</button>
              </Link>
            </div>

            <div className='userReviews item'>
              <h1>Most Recent Comments</h1>
              {commentItems.length > 0 ? commentItems : <p>No comments found.</p>}
              <Link className="mb-3" to={"Comments/" + userData.id}>
                <button>See all Comments</button>
              </Link>
            </div>

          </div>
        </section >
        :
        <h1>User not found</h1>
      }
    </>
  )
}