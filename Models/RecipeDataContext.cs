using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone.Models
{
    public class RecipeDataContext : DataContext
    {
        // Pass the connection string to the base class.
        public RecipeDataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<RecipeModel> Recipes;

        public Table<RecipeIngredient> Ingredients;

        public Table<RecipeTitle> Titles;

        public Table<RecipeContent> Contents;

        public Table<RecipeDescription> Descriptions;
    }

    [Table]
    public class RecipeModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int _recipeId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int RecipeId
        {
            get { return _recipeId; }
            set
            {
                if (_recipeId != value)
                {
                    NotifyPropertyChanging("RecipeId");
                    _recipeId = value;
                    NotifyPropertyChanged("RecipeId");
                }
            }
        }

        // Assign handlers for the add and remove operations, respectively.
        public RecipeModel()
        {
            Title = new RecipeTitle();
            Description = new RecipeDescription();

            _ingredients = new EntitySet<RecipeIngredient>(
                new Action<RecipeIngredient>(this.attach_Ingredient),
                new Action<RecipeIngredient>(this.detach_Ingredient)
                );

            _contents = new EntitySet<RecipeContent>(
                new Action<RecipeContent>(this.attach_Content),
                new Action<RecipeContent>(this.detach_Content)
                );
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        //#region RecipePhoto
        //[Column]
        //internal int _recipePhotoId;

        //private WriteableBitmap recipePhoto;
        //#endregion




        #region RecipeTitle
        // Internal column for the associated ToDoCategory ID value
        [Column]
        internal int _recipeTitleId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<RecipeTitle> _recipeTitle;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_recipeTitle", ThisKey = "_recipeTitleId", OtherKey = "BaseId", IsForeignKey = true)]
        public RecipeTitle Title
        {
            get { return _recipeTitle.Entity; }
            set
            {
                NotifyPropertyChanging("RecipeTitle");
                _recipeTitle.Entity = value;

                if (value != null)
                {
                    _recipeTitleId = value.BaseId;
                }

                NotifyPropertyChanging("RecipeTitle");
            }
        }
        #endregion

        #region RecipeDescription
        // Internal column for the associated ToDoCategory ID value
        [Column]
        internal int _recipeDescriptionId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<RecipeDescription> _recipeDescription;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_recipeDescription", ThisKey = "_recipeDescriptionId", OtherKey = "BaseId", IsForeignKey = true)]
        public RecipeDescription Description
        {
            get { return _recipeDescription.Entity; }
            set
            {
                NotifyPropertyChanging("Description");
                _recipeDescription.Entity = value;

                if (value != null)
                {
                    _recipeDescriptionId = value.BaseId;
                }

                NotifyPropertyChanging("Description");
            }
        }
        #endregion

        #region RecipeIngredient
        // Define the entity set for the collection side of the relationship.
        private EntitySet<RecipeIngredient> _ingredients;

        [Association(Storage = "_ingredients", OtherKey = "BaseId", ThisKey = "RecipeId")]
        public EntitySet<RecipeIngredient> Ingredients
        {
            get { return this._ingredients; }
            set { this._ingredients.Assign(value); }
        }

        // Called during an add operation
        private void attach_Ingredient(RecipeIngredient recipeIngredient)
        {
            NotifyPropertyChanging("RecipeIngredient");
            recipeIngredient.Recipe = this;
        }

        // Called during a remove operation
        private void detach_Ingredient(RecipeIngredient toDo)
        {
            NotifyPropertyChanging("RecipeIngredient");
            toDo.Recipe = null;
        }
        #endregion

        #region RecipeContent
        // Define the entity set for the collection side of the relationship.
        private EntitySet<RecipeContent> _contents;

        [Association(Storage = "_contents", OtherKey = "BaseId", ThisKey = "RecipeId")]
        public EntitySet<RecipeContent> Contents
        {
            get { return this._contents; }
            set { this._contents.Assign(value); }
        }

        // Called during an add operation
        private void attach_Content(RecipeContent recipeContent)
        {
            NotifyPropertyChanging("RecipeContent");
            recipeContent.Recipe = this;
        }

        // Called during a remove operation
        private void detach_Content(RecipeContent recipeContent)
        {
            NotifyPropertyChanging("RecipeContent");
            recipeContent.Recipe = null;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    [Table]
    public class RecipeTitle : RecipeBase
    {
        #region Text

        [Column]
        public override string Text
        {
            get { return _text; }
            set
            {
                NotifyPropertyChanging("Text");
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
        #endregion

        #region Id
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int BaseId
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        #endregion
    }

    [Table]
    public class RecipeIngredient : RecipeBase
    {
        #region Text
        [Column]
        public override string Text
        {
            get { return _text; }
            set
            {
                NotifyPropertyChanging("Text");
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
        #endregion

        #region Id
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int BaseId
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        #endregion

        #region Recipe
        // Internal column for the associated ToDoCategory ID value
        [Column]
        internal int _recipeId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<RecipeModel> _recipe;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_recipe", ThisKey = "_recipeId", OtherKey = "RecipeId", IsForeignKey = true)]
        public RecipeModel Recipe
        {
            get { return _recipe.Entity; }
            set
            {
                NotifyPropertyChanging("Recipe");
                _recipe.Entity = value;

                if (value != null)
                {
                    _recipeId = value.RecipeId;
                }

                NotifyPropertyChanging("Recipe");
            }
        }
        #endregion
    }

    [Table]
    public class RecipeDescription : RecipeBase
    {
        #region Text
        [Column]
        public override string Text
        {
            get { return _text; }
            set
            {
                NotifyPropertyChanging("Text");
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
        #endregion

        #region Id
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int BaseId
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        #endregion
    }

    [Table]
    public class RecipeContent : RecipeBase
    {
        #region Text
        [Column]
        public override string Text
        {
            get { return _text; }
            set
            {
                NotifyPropertyChanging("Text");
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }
        #endregion

        #region Id
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int BaseId
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
        #endregion

        #region Recipe
        // Internal column for the associated ToDoCategory ID value
        [Column]
        internal int _recipeId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<RecipeModel> _recipe;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_recipe", ThisKey = "_recipeId", OtherKey = "RecipeId", IsForeignKey = true)]
        public RecipeModel Recipe
        {
            get { return _recipe.Entity; }
            set
            {
                NotifyPropertyChanging("Recipe");
                _recipe.Entity = value;

                if (value != null)
                {
                    _recipeId = value.RecipeId;
                }

                NotifyPropertyChanging("Recipe");
            }
        }
        #endregion
    }
}
