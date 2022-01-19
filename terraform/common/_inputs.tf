
variable "object_prefix" {
  type = string
  validation {
    condition = contains(["dev", "prod"], var.object_prefix)
    error_message = "The object prefix must be set to `dev` or `prod`."
  }
}