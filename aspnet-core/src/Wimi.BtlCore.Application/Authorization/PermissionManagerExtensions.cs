namespace Wimi.BtlCore.Authorization
{
    using Abp.Authorization;
    using Abp.Runtime.Validation;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class PermissionManagerExtensions
    {
        /// <summary>
        /// Gets all permissions by names.
        ///     Throws <see cref="AbpValidationException"/> if can not find any of the permission names.
        /// </summary>
        /// <param name="permissionManager">
        /// The permission Manager.
        /// </param>
        /// <param name="permissionNames">
        /// The permission Names.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static IEnumerable<Permission> GetPermissionsFromNamesByValidating(
            this IPermissionManager permissionManager,
            IEnumerable<string> permissionNames)
        {
            var permissions = new List<Permission>();
            var undefinedPermissionNames = new List<string>();

            foreach (var permissionName in permissionNames)
            {
                var permission = permissionManager.GetPermissionOrNull(permissionName);
                if (permission == null)
                {
                    undefinedPermissionNames.Add(permissionName);
                }

                permissions.Add(permission);
            }

            if (undefinedPermissionNames.Count > 0)
            {
                throw new AbpValidationException(
                    $"There are {undefinedPermissionNames.Count} undefined permission names.")
                {
                    ValidationErrors
                                  =
                                  undefinedPermissionNames.ConvertAll(
                                      permissionName => new ValidationResult("Undefined permission: " + permissionName))
                };
            }

            return permissions;
        }
    }
}
