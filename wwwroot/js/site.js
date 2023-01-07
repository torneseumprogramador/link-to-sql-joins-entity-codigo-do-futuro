// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const app = {}
app.carregaModelosJQuey = (select) =>{
    $.ajax({
        url: "/modelos.json?marcaId=" + select.value
    }).done(function(dados) {
        var html = ""
        for(let i=0; i<dados.length; i++){
            html += `<option value="${dados[i].id}">${dados[i].nome}</option>`
        }
        $("#ModeloId").html(html);
    });
}

app.carregaModelosJsVanilla = async (select) =>{
    const dados = await (await fetch("/modelos.json?marcaId=" + select.value)).json();
    debugger
    var html = ""
    for(let i=0; i<dados.length; i++){
        html += `<option value="${dados[i].id}">${dados[i].nome}</option>`
    }
    document.getElementById("ModeloId").innerHTML = html;
}