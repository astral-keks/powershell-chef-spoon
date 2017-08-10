chef_server             = 
user_node               = 
validation_node         = 

workplace_dir           = File.join(File.dirname(__FILE__), '..')
secure_dir              = File.join(worplace_dir, 'Secure')

# Most likely these values will not be changed
chef_server_url           chef_server
cookbook_path             [""#{workplace_dir}/Source/trunk/Configuration/chef/cookbooks""]

node_name user_node
client_key                ""#{secure_dir}/#{user_node}.pem""

validation_client_name validation_node
validation_key            ""#{secure_dir}/#{validation_node}.pem""

log_level                 :info
log_location              STDOUT
ssl_verify_mode           :verify_none
knife[:editor] = '""c:\Program Files (x86)\Git\share\vim\vim73\vim.exe""'
cache_type                ""BasicFile""
cache_options( 
    :path               =>""#{workplace_dir}/Temp/chef/cache"" 
)
