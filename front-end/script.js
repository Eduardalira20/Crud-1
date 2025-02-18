const API_URL = "https://localhost:7226/api/Tasks";

// Pegar tarefas do backend
async function fetchTasks() {
    try {
        const response = await fetch(API_URL);
        const tasks = await response.json();
        const taskList = document.getElementById("taskList");
        taskList.innerHTML = "";  // Limpa a lista de tarefas antes de adicionar as novas
        tasks.forEach(task => {
            const li = document.createElement("li");

            // Adiciona os botões de Atualizar e Remover
            li.innerHTML = `${task.name} 
                            <button onclick="putTask(${task.id})">Atualizar</button> 
                            <button onclick="deleteTask(${task.id})">Remover</button>`;
            
            taskList.appendChild(li);
        });
    } catch (error) {
        console.error("Erro ao buscar tarefas:", error);
    }
}

// Adicionar nova tarefa
async function addTask() {
    const taskInput = document.getElementById("taskInput");
    const newTask = { name: taskInput.value, isCompleted: false };

    try {
        await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newTask)
        });
        taskInput.value = "";  // Limpa o campo de entrada
        fetchTasks();  // Atualiza a lista de tarefas
    } catch (error) {
        console.error("Erro ao adicionar tarefa:", error);
    }
}

// Remover tarefa
async function deleteTask(id) {
    try {
        await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        fetchTasks();  // Atualiza a lista de tarefas
    } catch (error) {
        console.error("Erro ao remover tarefa:", error);
    }
}

// Atualizar tarefa
async function putTask(id) {
    // Você pode criar uma forma mais adequada de alterar o nome aqui
    // Por exemplo, criando uma interface para isso, como um input ou modal.
    const newName = prompt("Digite o novo nome para a tarefa:");
    
    if (newName) {
        const updatedTask = { name: newName, isCompleted: false };

        try {
            await fetch(`${API_URL}/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(updatedTask)
            });
            fetchTasks();  // Atualiza a lista de tarefas
        } catch (error) {
            console.error("Erro ao atualizar tarefa:", error);
        }
    }
}

// Carregar tarefas ao abrir a página
fetchTasks();
