import React, { useState, useEffect, useRef } from 'react';
import { CKEditor } from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Link, useNavigate, useParams } from 'react-router-dom';

export default function EditRecipe() {
  const [bodyData, setBodyData] = useState('');
  const [oldBodyData, setOldBodyData] = useState('');
  const [titleData, setTitleData] = useState('');
  const [error, setError] = useState([]);
  const [tagString, setTagString] = useState("");
  const [loading, setLoading] = useState(0)
  const navigate = useNavigate();
  const { id } = useParams();

  useEffect(() => {
    fetch(`https://localhost:7147/api/Recipe/${id}`, {
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
        setTitleData(data.title);
        setOldBodyData(data.body);
        setBodyData(data.body);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });

    fetch(`https://localhost:7147/api/Recipe/RecipeTag/String?recipeId=${id}`, {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.text();
      })
      .then(data => {
        setTagString(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });

  }, [])

  const handleEditorChange = (event, editor) => {
    const data = editor.getData();
    setBodyData(data);
  };

  const handleTitleChange = (e) => {
    setTitleData(e.target.value);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();

    console.log(titleData);
    console.log(bodyData);
    console.log(tagString);

    const currentDate = new Date();

    const jsonData = {
      "id": id,
      "title": titleData,
      "body": bodyData,
      "createdDateTime": currentDate.toISOString(),
      "authorId": null,
      "author": null
    }

    fetch(`https://localhost:7147/api/Recipe/${id}`, {
      method: 'PUT',
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
        navigate('/Recipes/Detail/' + id);
      })
      .catch(error => {
        console.error('Error updating recipe:', error.message);
        throw error;
      });

    console.log(jsonData);
  };


  useEffect(() => {
    const initializeEditor = async () => {
      const editor = await ClassicEditor.create(document.querySelector('#editor'), {
        ckbox: {
          tokenUrl: "https://localhost:7147/api/CKBox/token",
          theme: 'lark'
        },
        removePlugins: ['FormatPainter', 'Table'],
      });

      editor.model.document.on('change:data', () => {
        handleEditorChange(null, editor);
      });

      editor.setData(oldBodyData);
    };

    if (loading != 0) {
      initializeEditor();
    }
    setLoading(1);
  }, [oldBodyData]);

  return (
    <section className='editRecipeContainer'>
      <section>
        <section style={{ height: "90px" }}></section>
        <section className='heroContainer' style={{ background: "#f7f7f7" }}>
          <div className="m-auto text-center p-5 pb-3 w-60">
            <h1 className="mb-5">Edit recipe</h1>
            <h5 className='text-secondary w-75 m-auto text-justify'>Found a better choice for an ingredient? Discovered a new secret trick, passed on generation by generation on how to better this recipe? Whatever it is, we thank you for keeping your recipe up to date.</h5>
            <div className='mt-5'></div>
            <div className="form-group">
              <div className="input-group">
              </div>
            </div>
          </div>
        </section>

        <div className='w-60 m-auto pt-5 overflow-hidden'>
          <div className='rotator d-flex overflow-hidden w-100' style={{ transition: "0.3s all ease-in" }}>
            <div className='rotatingItem w-100'>
              <div>
                <input type="text" placeholder='Title' className="form-control rounded-0 me-2 mb-2" value={titleData} onChange={handleTitleChange} />
              </div>
              <div id="editor" />
            </div>
          </div>

          <form onSubmit={handleFormSubmit}>
            <button type='submit' className={`btn btn-primary`} style={{ float: "right" }}>
              Submit Recipe <i className="bi bi-plus-square"></i>
            </button>
          </form>
        </div>
      </section>
    </section>
  );
}
