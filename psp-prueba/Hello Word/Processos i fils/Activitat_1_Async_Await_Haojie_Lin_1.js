async function f(){

    let mysql = require("mysql");
    
    let connection = mysql.createConnection({
        host:"localhost",
        database:"async_await",
        user:"root",
        password:""
    });
    
    connection.connect(function(err){
        if(err){
            throw err;
        }
        else{
            console.log("Conectado");
        }
    });
    
    console.log("1");
    
    console.log("2");

    const categorias = "SELECT * FROM `material`;"
    connection.query(categorias,function(error,lista){
        if(error){
            throw error;
        }
        else{
            console.log(lista);
        }
    })
    
    connection.end();
    }
    
    f();