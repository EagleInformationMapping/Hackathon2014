from settings import *
DEBUG = True
TEMPLATE_DEBUG = DEBUG
#DTt0NsUj9LpX
DATABASES = {
   'default': {
       'ENGINE': 'sql_server.pyodbc',
       'NAME': 'hackathon2014',
       'USER': 'hacker@vkrat0u6vm',
       'PASSWORD': 'DTt0NsUj9LpX',
       'HOST': 'vkrat0u6vm.database.windows.net',
       'PORT': '',
       'OPTIONS': {
           'driver': 'SQL Server Native Client 11.0',
           'MARS_Connection': True,
       },
   }
}