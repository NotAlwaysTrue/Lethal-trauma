
let blockConfigSending = false
let serverID;

Main()



async function Main() {
  AddReactivity()
  await GetConfig()
  await GetServerID()

  WatchServer()
}



async function GetServerID() {
  let responce = await fetch(`${window.location.origin}/GetServerID`)
  serverID = await responce.text()
}

async function WatchServer() {
  setInterval(async () => {
    try {
      let responce = await fetch(`${window.location.origin}/GetServerID`)
      let id = await responce.text()

      if (id != serverID) {
        serverID = id
        OnServerChanged()
      }
    } catch (error) {
      console.log(error)
    }
  }, 2000)
}

function OnServerChanged() {
  console.log(`server has changed`)
  GetConfig()
}

function debounce(callback, wait) {
  let timeoutId = null;
  return (...args) => {
    window.clearTimeout(timeoutId);
    timeoutId = window.setTimeout(() => {
      callback(...args);
    }, wait);
  };
}

OnSettingChanged = debounce((key, value) => {
  SendConfig()
}, 100)


function ApplyConfigToForm(config) {
  let form = document.forms.settings

  for (const element of form.elements) {
    if (element.type == 'number') {
      element.value = config[element.name]
    }
    if (element.type == 'checkbox') {
      element.checked = config[element.name]
    }
  }
}

function ExtractConfigFromForm() {
  let form = document.forms.settings

  let config = {}

  for (const element of form.elements) {
    if (element.type == 'number') {
      config[element.name] = parseFloat(element.value)
    }
    if (element.type == 'checkbox') {
      config[element.name] = element.checked
    }
  }

  return config
}

function AddReactivity() {
  let form = document.forms.settings

  for (const element of form.elements) {
    if (element.type == 'number') {
      element.oninput = () => {
        if (!element.validity.valid) return
        OnSettingChanged(element.name, parseFloat(element.value))
      }
    }
    if (element.type == 'checkbox') {
      element.oninput = () => {
        if (!element.validity.valid) return
        OnSettingChanged(element.name, element.checked)
      }
    }
  }
}




async function GetConfig() {
  blockConfigSending = true
  console.log(`getting the config`)
  let responce = await fetch(`${window.location.origin}/GetConfig`)
  ApplyConfigToForm(await responce.json())
  blockConfigSending = false
}

async function SendConfig() {
  if (blockConfigSending) return

  console.log(`sending the config`)

  let responce = await fetch(`${window.location.origin}/SetConfig`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(ExtractConfigFromForm())
  });
}