


let config = {}

Main()

async function Main() {
  await GetConfig()
  config.RangedWeaponDamage = 123

  await SetConfig()
}

async function GetConfig() {
  let responce = await fetch(`${window.location.origin}/GetConfig`)
  config = await responce.json()
}

async function SetConfig() {
  let responce = await fetch(`${window.location.origin}/SetConfig`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(config)
  });
}