
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace TaskManager
{
    class Program
    {
        //Create a list to store all the categories in the task manager
        static List<Category> categories = new List<Category>();

        //A flag to indicate whether the table needs to be updated
        static bool tableNeedsUpdate = true;

        static void Main(string[] args)
        {
            //calling the method to setup the default categories
            InitializeDefaultCategories();

            while (true)
            {
                if (tableNeedsUpdate)
                {
                    //clear the connsole window
                    Console.Clear();

                    //calls the display table method to display the table on console
                    DisplayTable();
                    tableNeedsUpdate = false;
                    // Since the table is now up-to-date resets the flag.
                }

                //display the menu options to the user
                DisplayMenu();

                //processes user input and carries out the appropriate action
                HandleMenuSelection();
                
            }
        }

        static void InitializeDefaultCategories()
        {
            //creates the three default categories personal, work and family
            categories.Add(new Category("Personal"));
            categories.Add(new Category("Work"));
            categories.Add(new Category("Family"));
        }

        static void DisplayTable()
        {
            DisplayCategoryHeaders();   //display category headers
            DisplayTasks();     //display tasks in each category
        }

        static void DisplayCategoryHeaders()
        {
            int columnWidth = 40;   //width for category columns
            int idWidth = 5;    //width for ID column

            //creates a string with 12 spaces to the left of the word CATEGORIES
            Console.WriteLine(new string(' ', 12) + "CATEGORIES");

            //draws a line of _ based on column widths and number of categories
            Console.WriteLine(new string(' ', 10) + new string('-', (columnWidth + 1) * categories.Count + idWidth + 1));

            //pads the string ID to the right
            Console.Write($"{"ID".PadRight(idWidth)}|");

            //loops through each category
            foreach (var category in categories)
            {
                //pads the category name to fit the width and truncates if necessary
                string categoryName = category.Name.PadRight(columnWidth).Substring(0, columnWidth);
                //displays category name
                Console.Write($"{categoryName}|");
            }
            Console.WriteLine();
        }

        static void DisplayTasks()
        {
            int columnWidth = 40;
            int idWidth = 5;
            // Finds the maximum number of tasks in categories
            int maxTasks = categories.Max(c => c.Tasks.Count);

            //loops through each task till max task
            for (int i = 0; i < maxTasks; i++)
            {
                //The current task index i is converted to a string and padded to match the width of the ID column
                // Displays the task ID
                Console.Write($"{i.ToString().PadRight(idWidth)}|");

                //loops through each category
                foreach (var category in categories)
                {
                    //determines if the current task index i falls under the category's acceptable range of tasks
                    if (i < category.Tasks.Count)
                    {
                        //gets the task at index i
                        var task = category.Tasks[i];
                        //Calls the Display method to display the task
                        task.Display(columnWidth);
                    }
                    else
                    {
                        //if there is no task at the current index, fills in the blank cells
                        Console.Write($"{"".PadRight(columnWidth)}|");
                    }
                }
                Console.WriteLine();
            }
        }


        static void DisplayMenu()
        {
            //prints Menu as header
            Console.WriteLine("\nMenu:");

            //iterates over each option in the ActionType enum
            foreach (var action in Enum.GetValues(typeof(ActionType)))
            {
                //prints each menu option in the enum
                Console.WriteLine($"{(int)action}. {action}");
            }
            Console.Write("Please enter your choice: ");
        }

        static void HandleMenuSelection()
        {
            //converts the user input into an `ActionType` enum
            if (Enum.TryParse<ActionType>(Console.ReadLine(), out ActionType choice))
            {
                //switches based on ActionType
                switch (choice)
                {
                    case ActionType.AddTask:
                        HandleAddTask(); // Calls the method to handle adding a task
                        break;

                    case ActionType.DeleteTask:
                        HandleDeleteTask(); // Calls the method to handle deleting a task
                        break;

                    case ActionType.MoveTaskPriority:
                        HandleMovePriority(); // Calls the method to handle moving task priority
                        break;

                    case ActionType.MoveTaskToDifferentCategory:
                        HandleMoveTask();  // Calls the method to handle moving a task to different category
                        break;

                    case ActionType.HighlightTask:
                        HandleHighlightTask();  // Calls the method to handle highlighting a task
                        break;

                    case ActionType.AddCategory:
                        HandleAddCategory();  //Calls the method to handle adding a category
                        break;

                    case ActionType.DeleteCategory:
                        HandleDeleteCategory();  // Calls the method to handle deleting a category
                        break;

                    case ActionType.Exit:
                        Environment.Exit(0);  //exits the program
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again."); ////if an invalid option is chosen, prints an error
                        break;
                }
            }
            else
            {
                //if an invalid input is given, prints an error
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        static void HandleAddTask()
        {
            // gets category object based on user input
            var category = GetCategoryByName();

            //checks if a valid category was retrieved
            if (category != null)
            {
                //calls AddTask method to add a new task
                category.AddTask();
                tableNeedsUpdate = true; //updates the table
            }
        }

        static void HandleDeleteTask()
        {
            // gets category object based on user input
            var category = GetCategoryByName();

            //checks if a valid category was retrieved
            if (category != null)
            {
                //calls DeleteTask method to delete a task
                category.DeleteTask();
                tableNeedsUpdate = true;
            }
        }

        static void HandleMovePriority()
        {
            // gets category object based on user input
            var category = GetCategoryByName();

            //checks if a valid category was retrieved
            if (category != null)
            {
                //calls MoveTaskPriority method to change the priority of a task
                category.MoveTaskPriority();
                tableNeedsUpdate = true;
            }
        }

        static void HandleMoveTask()
        {
            // gets source category object based on user input
            var sourceCategory = GetCategoryByName("source");

            //if no category is found exit the method
            if (sourceCategory == null) return;

            Console.Write("Enter task ID to move: ");

            // convert the user's input into an integer
            // check if it is a valid task ID in the source category
            if (int.TryParse(Console.ReadLine(), out int taskId) && sourceCategory.IsValidTaskId(taskId))
            {
                // gets destination category object based on user input
                var destinationCategory = GetCategoryByName("destination");

                //if a destination category is found move the task
                if (destinationCategory != null)
                {
                    //move the task from the source category to the destination category using the task ID
                    sourceCategory.MoveTaskToCategory(destinationCategory, taskId);
                    tableNeedsUpdate = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid task ID.");
            }
        }

        static void HandleHighlightTask()
        {
            // gets category object based on user input
            var category = GetCategoryByName();

            //checks if a valid category was retrieved
            if (category != null)
            {
                //calls HighlightTask method to highlight a task
                category.HighlightTask();
                tableNeedsUpdate = true;
            }
        }

        static void HandleAddCategory()
        {
            Console.Write("Enter new category name: ");
            string newCategoryName = Console.ReadLine().Trim();

            //checks if a category with the same name already exists in the 'categories' list
            //ignores the case
            if (!categories.Any(c => c.Name.Equals(newCategoryName, StringComparison.OrdinalIgnoreCase)))
            {
                //adds a new category to the categories list
                categories.Add(new Category(newCategoryName));
                tableNeedsUpdate = true;    //updates the table
                Console.WriteLine("Category added successfully.");
            }
            else
            {
                Console.WriteLine("Category already exists.");
            }
        }

        static void HandleDeleteCategory()
        {
            // gets category object based on user input
            var category = GetCategoryByName();

            //checks if a valid category was retrieved
            if (category != null)
            {
                //remove the category from the categories list
                categories.Remove(category);
                tableNeedsUpdate = true;    //updates the table
                Console.WriteLine("Category and its tasks deleted successfully.");
            }
        }

        static Category GetCategoryByName(string prompt = "category")
        {
            // propmts the user to enter a category name.
            Console.Write($"Enter {prompt} name: ");
            string categoryName = Console.ReadLine().Trim();

            //using the FirstOrDefault LINQ method to search for the first item in the categories collection
            //that satisfies the condition in the lambda expression where the category name matches categoryName
            //Searches for a category with the input name ignoring the case
            var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                //prints an error message if category was not found
                Console.WriteLine($"{prompt} not found.");
            }

            return category;
        }
    }


    enum ActionType
    {
        AddTask = 1,
        DeleteTask,
        MoveTaskPriority,
        MoveTaskToDifferentCategory,
        HighlightTask,
        AddCategory,
        DeleteCategory,
        Exit
    }


    class Category
    {
        //property for the category name, only settable within the class
        public string Name { get; private set; }
        //private list to store tasks within the category
        private List<Task> tasks = new List<Task>();

        // Read-only access to tasks
        public List<Task> Tasks => tasks; 

        //Constructor

        public Category(string name)
        {
            Name = name;
        }

        public void AddTask()
        {
            Console.Write("Enter task description (max 30 characters): ");
            string description = Console.ReadLine();
            // Trims the description to 30 characters if necessary
            description = description.Length > 30 ? description.Substring(0, 30) : description;

            Console.Write("Enter due date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                //adds a new task to the list with task description and due date
                tasks.Add(new Task(description, dueDate));
                Console.WriteLine("Task added successfully.");
            }
            else
            {
                // Prints an error if the date format is incorrect.
                Console.WriteLine("Invalid date format.");
            }
        }

        public void DeleteTask()
        {
            //asks user for the task ID of the task they want to delete
            Console.Write("Enter task ID to delete: ");

            if (int.TryParse(Console.ReadLine(), out int taskId) && IsValidTaskId(taskId))
            {
                //deletes the task from given task ID
                tasks.RemoveAt(taskId);
                Console.WriteLine("Task deleted successfully.");
            }
            else
            {
                //prints error if an invalid task ID is given
                Console.WriteLine("Invalid task ID.");
            }
        }

        public void MoveTaskPriority()
        {
            Console.Write("Enter current task ID: ");

            //converts the user input as an integer stores in currentId
            if (int.TryParse(Console.ReadLine(), out int currentId) && IsValidTaskId(currentId))
            {
                Console.Write("Enter new priority position: ");

                //converts the user input as an integer stores in newposition 
                if (int.TryParse(Console.ReadLine(), out int newPosition) && newPosition >= 0 && newPosition < tasks.Count)
                {
                    //retrieve the task at the current position
                    var task = tasks[currentId];
                    tasks.RemoveAt(currentId);  //remove the task form current position

                    //adds the task to new positioon given
                    tasks.Insert(newPosition, task);
                    Console.WriteLine("Task priority updated successfully.");
                }
                else
                {
                    //prints error if invalid task ID is given for new position
                    Console.WriteLine("Invalid new position.");
                }
            }
            else
            {
                //prints error if invalid task ID is given
                Console.WriteLine("Invalid task ID.");
            }
        }

        public void MoveTaskToCategory(Category destinationCategory, int taskId)
        {
            //retrieve the task from the current task ID
            var task = tasks[taskId];
            tasks.RemoveAt(taskId); //remove the task from the current index

            destinationCategory.tasks.Add(task);   //adds the task to the destination category
            Console.WriteLine("Task moved successfully.");
        }

        public void HighlightTask()
        {
            Console.Write("Enter task ID to highlight: ");

            //convert the input to an intege and stores the value in taskId
            if (int.TryParse(Console.ReadLine(), out int taskId) && IsValidTaskId(taskId))
            {
                //toggles the importance of a task at the given task ID
                tasks[taskId].ToggleImportance();
                Console.WriteLine("Task highlight status changed.");
            }
            else
            {
                //if task Id is ivalid writes error message
                Console.WriteLine("Invalid task ID.");
            }
        }

        public bool IsValidTaskId(int taskId)
        {
            //ensures that a provided taskId is a valid index
            return taskId >= 0 && taskId < tasks.Count;
        }

        //returns an IEnumerable<Task> containing the whole list of tasks in this category
        //this enable other parts of the program to loop over the tasks without changing the initial list

        public IEnumerable<Task> GetAllTasks() => tasks;

        //Provides a way to access only the tasks marked as important
        //using LINQ - .Where(...) to filter the tasks based on the IsImportant property
        //t represents each individual Task object in the tasks list

        public IEnumerable<Task> GetImportantTasks() => tasks.Where(t => t.IsImportant);
    }


    class Task
    {
        private string description;
        private DateTime dueDate;
        private bool isImportant;

        public string Description
        {
            //retrives the description field's value
            get => description;

            //allows the description field's value to be modified
            set
            {
                //checks if the vlue length is less than or equal to 30 characters
                if (value.Length <= 30)
                {
                    //if true, it assigns value to the description
                    description = value;
                }
                else
                {
                    //if longer than 30 characters
                    //takes the first 30 characters of the string and assigns them to description
                    description = value.Substring(0, 30);
                }
            }
        }

        // Property to access due date
        public DateTime DueDate
        {
            get => dueDate;
            set => dueDate = value;
        }

        // Read-only property to check if the task is important
        public bool IsImportant => isImportant;

        //Constructor

        public Task(string description, DateTime dueDate)
        {
            Description = description;    
            DueDate = dueDate;
            isImportant = false;
        }

        public void Display(int columnWidth)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            // Checks if the task is marked as important
            if (isImportant)
            {
                //if marked important changes colr to red
                Console.ForegroundColor = ConsoleColor.Red;
            }

            //creates a string that includes the task description and due date
            //converts the DueDate property to a short date string format - 12/31/2024
            string taskDisplay = $"{Description} (Due: {DueDate.ToShortDateString()})";
            taskDisplay = taskDisplay.PadRight(columnWidth).Substring(0, columnWidth);

            //writes the taskDisplay string to the console
            Console.Write($"{taskDisplay}|");

            //resets to the original console color
            Console.ForegroundColor = originalColor;
        }

        //switches the importance status of a task
        public void ToggleImportance() => isImportant = !isImportant;
    }
}

