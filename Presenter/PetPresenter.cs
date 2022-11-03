using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDWinFormsMVP.Models;
using CRUDWinFormsMVP.Views;
using CRUDWinFormsMVP._Repositories;
using System.Drawing;


namespace CRUDWinFormsMVP.Presenter
{
    public class PetPresenter
    {
        //Fields
        private IPetView view;
        private IPetRepository repository;
        private BindingSource petsBindingSource;
        private IEnumerable<PetModel> petList;

        public PetPresenter(IPetView view, IPetRepository repository)
        {
            this.petsBindingSource = new BindingSource();
            this.view = view;
            this.repository = repository;
            //subscribe event handler methods to view events
            this.view.SearchEvent += SearchPet;
            this.view.AddNewEvent += AddNewPet;
            this.view.EditEvent += LoadSelectedPetToEdit;
            this.view.DeleteEvent += DeleteSelectedPet;
            this.view.SaveEvent += SavePet;
            this.view.CancelEvent += CancelAction;
            ///Set pets binding source
            this.view.SetPetListBindingSource(petsBindingSource);
            LoadAllPetList();
            //show view
            this.view.Show();
        }

        private void LoadAllPetList()
        {
            petList = repository.GetAll();
            petsBindingSource.DataSource = petList; //set data source
        }
        private void SearchPet(object? sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.view.SearchValue);
            if(emptyValue == false)
            {
                petList = repository.GetByValue(this.view.SearchValue);
                petsBindingSource.DataSource = petList;
            }
            else
            {
                LoadAllPetList();
            }
        }

        private void CancelAction(object? sender, EventArgs e)
        {
            CleanViewFields();
        }

        private void SavePet(object? sender, EventArgs e)
        {
            var model = new PetModel();
            model.Id = Convert.ToInt32(view.PetId);
            model.Name = view.PetName;
            model.Type = view.PetType;
            model.Colour = view.PetColour;
            try
            {
                new Common.ModelDataValidation().Validate(model);
                if (view.IsEdit) // edit model
                {
                    repository.Edit(model);
                    view.Message = "Pet edited succesfully";
                }
                else
                {
                    repository.Add(model);
                    view.Message = "Pet added succesfully";
                }
                view.IsSuccesful = true;
                LoadAllPetList();
                CleanViewFields();

            }
            catch (Exception ex)
            {
                view.IsSuccesful = false;
                view.Message = ex.Message;
            }

        }

        private void CleanViewFields()
        {
            view.PetId = "0";
            view.PetName = "";
            view.PetType = "";
            view.PetColour = "";
        }

        private void DeleteSelectedPet(object? sender, EventArgs e)
        {
            try
            {
                var pet = (PetModel)petsBindingSource.Current;
                repository.Delete(pet.Id);
                view.IsSuccesful = true;
                view.Message = "Pet deleted successfully";
                LoadAllPetList();

            }
            catch (Exception ex)
            {
                view.IsSuccesful = false;
                view.Message = "There was an error while deleting the pet.";
            }
        }

        private void LoadSelectedPetToEdit(object? sender, EventArgs e)
        {
            var pet = (PetModel)petsBindingSource.Current;
            view.PetId = pet.Id.ToString();
            view.PetName = pet.Name;
            view.PetType = pet.Type;
            view.PetColour = pet.Colour;
            view.IsEdit = true;


        }

        private void AddNewPet(object? sender, EventArgs e)
        {
            view.IsEdit = false;
        }

       
    }
}
