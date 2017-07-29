# Cortana-Location-UWP
Hey, Cortana! Where am I?

# Steps to run
- Create file `/ComputerAssistant.UI/app.creds`
- Build Solution
- Publish ComputerAssistant.Bot to HTTPS endpoint
	- HINT: Publishing to Azure automatically provides an HTTPS endpoint
- Browse to https://dev.botframework.com/bots/
	- Create a bot
	- Add Direct Line Channel
- Update `Web.config` with the required settings
	- AppSetting Key = Bot Framework Value
	- BotId = Bot Handle
	- MicrosoftAppId = App ID
	- MicrosoftAppPassword = Password
- Update `app.creds`
	- Direct Line key so it is the only thing the file contains
