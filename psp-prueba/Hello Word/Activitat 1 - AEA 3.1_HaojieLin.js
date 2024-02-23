async function f(){

  let mysql = require("mysql");
  
  let connection = mysql.createConnection({
      host:"localhost",
      database:"async / await",
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
  })
  
  console.log("1");
  
  const categorias = "SELECT * FROM `material`;"
  connection.query(categorias,function(error,lista){
      if(error){
          throw error;
      }
      else{
          console.log(lista);
      }
  })
  
  console.log("2");
  
  connection.end();
  }
  
  f();