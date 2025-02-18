using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TaskApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography.X509Certificates;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost;Database=MeuCruddb;User=root;Password=ilgawejmp;";


        [HttpGet]
        public IActionResult GetTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Tasks";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            IsCompleted = reader.GetBoolean("is_completed")
                        });
                    }
                }
            }

            return Ok(tasks);
        }

        // Endpoint para testar conexão com o banco
        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return Ok("Conexão com o banco de dados foi bem-sucedida!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao conectar ao banco de dados: {ex}");
            }
        }


        [HttpPost]
        public IActionResult PostTask([FromBody] TaskItem task)
        {
            if (task == null)
            {
                return BadRequest("Tarefa não fornecida.");
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Cria uma query SQL para inserir os dados da tarefa
                    string query = "INSERT INTO Tasks (name, is_completed) VALUES (@name, @is_completed);";

                    // Executa o comando SQL
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", task.Name);
                        command.Parameters.AddWithValue("@is_completed", task.IsCompleted);

                        command.ExecuteNonQuery();

                        // Obtém o ID da tarefa recém-criada
                        command.CommandText = "SELECT LAST_INSERT_ID()";
                        task.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                // Retorna uma resposta CreatedAtAction, que inclui o novo ID da tarefa
                return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao adicionar a tarefa: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM tasks WHERE id = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound("Tarefa não encontrada.");


                    }
                }
                return NoContent(); // Retorna HTTP 204 (Sucesso, sem conteúdo)
            }



        }
        [HttpPut("{id}")]
        public IActionResult PutTask(int id,[FromBody] TaskItem updatedTask)
        {
            if (updatedTask == null) return BadRequest("Dados inválidos.");
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE Tasks SET name = @name, is_completed = @is_completed WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@name", updatedTask.Name);
                    command.Parameters.AddWithValue("@is_completed", updatedTask.IsCompleted);
                    command.Parameters.AddWithValue("@id", id);

                    return command.ExecuteNonQuery() == 0 
                        ? NotFound("Tarefa não encontrada.")
                        : NoContent();// Retorna HTTP 204 (Sucesso, sem conteúdo)
                }
               
            }



                
        }
        
    }
}

